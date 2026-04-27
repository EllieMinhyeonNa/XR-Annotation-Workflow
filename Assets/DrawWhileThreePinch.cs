using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class DrawWhileThreePinch : MonoBehaviour
{
    [Header("Hand Source")]
    public XRHandTrackingEvents handTrackingEvents; // Right/Left Hand Tracking

    [Header("Gesture")]
    public DetectGesture threePinchDetector;        // DetectGestures_Right/Left

    [Header("Pointer (Pencil)")]
    public Transform pencilRoot;                    // Pencil_R / Pencil_L
    public Transform pencilTip;                     // PencilRoot/Tip
    public Vector3 tipPositionOffset = new Vector3(0, 0, 0.02f);

    [Header("Drawing")]
    public LineRenderer linePrefab;                 // StrokePrefab
    public Transform strokeParent;                  // Parent for organizing strokes (optional)
    public float minDistance = 0.01f;
    public float lineWidth = 0.01f;

    [Header("Stabilization")]
    [Tooltip("0이면 필터 없음. 0.15~0.3 정도 추천 (값이 클수록 더 부드러움)")]
    public float rotationSmoothing = 0.2f;
    [Tooltip("펜의 로컬 축이 forward와 다르면 여기에 보정(예: (0,90,0))")]
    public Vector3 pencilModelEulerOffset = Vector3.zero;

    [Header("Grab Placement")]
    [Tooltip("엄지-검지 중간점에서 펜 루트를 얼마나 이동시킬지(펜 로컬 기준)")]
    public Vector3 gripLocalOffset = new Vector3(0f, 0f, 0.08f);

    [Tooltip("손가락 사이로 살짝 밀어넣는 정도(+면 palmNormal 방향으로)")]
    public float pinchInward = 0.005f;

    [Header("Latency Fix: Hard Stop")]
    [Tooltip("엄지-검지 거리(m)가 이 값보다 커지면 즉시 드로잉 종료 (0.03~0.06 튜닝)")]
    public float thumbIndexReleaseDistance = 0.04f;

    LineRenderer current;
    readonly List<Vector3> points = new();
    bool wasDrawingMode;

    Quaternion smoothedRotation = Quaternion.identity;
    bool hasSmoothedRotation;

    void OnEnable()  => handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
    void OnDisable() => handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

    void OnJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        bool drawingMode = threePinchDetector != null && threePinchDetector.IsDetected;
        if (!handTrackingEvents.handIsTracked) drawingMode = false;

// Hard stop: 제스처 판정이 늦게 풀려도 "엄지-검지 벌어지면" 즉시 종료
        if (drawingMode)
        {
            if (TryGetJointPose(args.hand, XRHandJointID.ThumbTip, out Pose thumbTipPose) &&
                TryGetJointPose(args.hand, XRHandJointID.IndexTip, out Pose indexTipPoseForStop))
            {
                float d = Vector3.Distance(thumbTipPose.position, indexTipPoseForStop.position);
                if (d > thumbIndexReleaseDistance)
                    drawingMode = false;
            }
        }

        if (drawingMode && !wasDrawingMode)
        {
            SetPencilVisible(true);
            BeginStroke();
        }
        else if (!drawingMode && wasDrawingMode)
        {
            SetPencilVisible(false);
            EndStroke();
            hasSmoothedRotation = false;

            wasDrawingMode = drawingMode;
            return;
        }

        wasDrawingMode = drawingMode;
        if (!drawingMode) return;

        // ===== 1) 필요한 관절 포즈 가져오기 =====
        if (!TryGetJointPose(args.hand, XRHandJointID.IndexTip, out Pose indexTipPose)) return;

        // 엄지/검지 방향 벡터를 위한 관절들
        // (너가 그린 것처럼) thumb 1->2, index 3->4 느낌을 구현:
        // thumb: ThumbMetacarpal -> ThumbTip
        // index: IndexProximal  -> IndexTip
        // 필요하면 이 두 줄만 다른 joint로 바꿔도 됨.
        if (!TryGetJointPose(args.hand, XRHandJointID.ThumbMetacarpal, out Pose thumbBase)) return;
        if (!TryGetJointPose(args.hand, XRHandJointID.ThumbTip,        out Pose thumbTip))  return;

        if (!TryGetJointPose(args.hand, XRHandJointID.IndexProximal,   out Pose indexBase)) return;
        if (!TryGetJointPose(args.hand, XRHandJointID.IndexTip,        out Pose indexTip2)) return;

        Vector3 thumbDir = (thumbTip.position - thumbBase.position);
        Vector3 indexDir = (indexTip2.position - indexBase.position);

        if (thumbDir.sqrMagnitude < 1e-6f || indexDir.sqrMagnitude < 1e-6f) return;

        thumbDir.Normalize();
        indexDir.Normalize();

        // ===== 2) 펜 forward: 엄지+검지의 "중간 방향" =====
        Vector3 forward = (thumbDir + indexDir);
        if (forward.sqrMagnitude < 1e-6f)
        {
            // 혹시 거의 반대 방향이면, 그냥 검지 방향으로 fallback
            forward = indexDir;
        }
        forward.Normalize();

        // ===== 3) 펜 up: "집는 면" 법선으로 롤 안정화 =====
        Vector3 palmNormal = Vector3.Cross(indexDir, thumbDir);
        if (palmNormal.sqrMagnitude < 1e-6f)
        {
            // 거의 일직선이면 fallback
            palmNormal = args.hand.rootPose.rotation * Vector3.up;
        }
        palmNormal.Normalize();

        // 오른손/왼손에 따라 법선 방향이 뒤집힐 수 있으니, forward 기준으로 한번 통일
        // (up이 forward와 너무 같은 방향이면 이상해지니까)
        if (Vector3.Dot(Vector3.Cross(forward, palmNormal), args.hand.rootPose.rotation * Vector3.up) < 0f)
            palmNormal = -palmNormal;

        Quaternion targetRot = Quaternion.LookRotation(forward, palmNormal);

        // 모델 축 보정(펜 프리팹이 forward가 +Z가 아닐 때)
        if (pencilModelEulerOffset != Vector3.zero)
            targetRot = targetRot * Quaternion.Euler(pencilModelEulerOffset);

        // ===== 4) 회전 스무딩(허둥거림 줄이기) =====
        if (!hasSmoothedRotation)
        {
            smoothedRotation = targetRot;
            hasSmoothedRotation = true;
        }
        else if (rotationSmoothing > 0f)
        {
            // smoothing 값이 클수록 더 부드럽게(느리게 따라감)
            float t = 1f - Mathf.Exp(-rotationSmoothing / Mathf.Max(Time.deltaTime, 1e-6f));
            smoothedRotation = Quaternion.Slerp(smoothedRotation, targetRot, t);
        }
        else
        {
            smoothedRotation = targetRot;
        }

        // ===== 5) 펜 위치: indexTip + offset (기존 방식 유지) =====
        Vector3 gripMid = (thumbTip.position + indexTip2.position) * 0.5f;
        UpdatePencilPose(gripMid, palmNormal, smoothedRotation);

        // ===== 6) 드로잉 포인트 =====
        Vector3 drawPoint = (pencilTip != null) ? pencilTip.position : indexTipPose.position;
        AddPoint(drawPoint);
    }

    static bool TryGetJointPose(XRHand hand, XRHandJointID id, out Pose pose)
    {
        pose = default;
        XRHandJoint j = hand.GetJoint(id);
        return j.TryGetPose(out pose);
    }

    void UpdatePencilPose(Vector3 gripMidPos, Vector3 palmNormal, Quaternion rot)
    {
        if (pencilRoot == null) return;

        // 1) 엄지-검지 중간점을 기본 앵커로
        Vector3 pos = gripMidPos;

        // 2) 손가락 사이로 살짝 밀어넣기 (집는 면 법선 방향)
        pos += palmNormal * pinchInward;

        // 3) 펜 로컬 기준으로 "잡는 위치"를 펜 몸통 가운데로 보정
        //    (즉, 루트가 그립 위치에 오도록 펜을 뒤로 빼거나 옆으로 미는 튜닝)
        pos += rot * gripLocalOffset;

        pencilRoot.SetPositionAndRotation(pos, rot);
    }

    void SetPencilVisible(bool visible)
    {
        if (pencilRoot != null) pencilRoot.gameObject.SetActive(visible);
    }

    void BeginStroke()
    {
        if (linePrefab == null) return;

        // Create line renderer with optional parent
        current = Instantiate(linePrefab, strokeParent);
        current.numCapVertices = 12;
        current.numCornerVertices = 12;
        current.useWorldSpace = true;
        current.startWidth = lineWidth;
        current.endWidth = lineWidth;

        points.Clear();
        current.positionCount = 0;
    }

    void AddPoint(Vector3 p)
    {
        if (current == null) return;
        if (points.Count > 0 && Vector3.Distance(points[^1], p) < minDistance) return;

        points.Add(p);
        current.positionCount = points.Count;
        current.SetPosition(points.Count - 1, p);
    }

    void EndStroke()
    {
        current = null;
        points.Clear();
    }
}


