// Copyright 2021-2022 The SeedV Lab.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Color = UnityEngine.Color;

using Mediapipe;
using Mediapipe.Unity;

namespace SeedUnityVRKit {
  public enum HandType { LeftHand, RightHand }

  public class HandLandmarks {
    public const int ThumbProximal = 0;
    public const int ThumbIntermediate = 1;
    public const int ThumbDistal = 2;
    public const int IndexProximal = 3;
    public const int IndexIntermediate = 4;
    public const int IndexDistal = 5;
    public const int MiddleProximal = 6;
    public const int MiddleIntermediate = 7;
    public const int MiddleDistal = 8;
    public const int RingProximal = 9;
    public const int RingIntermediate = 10;
    public const int RingDistal = 11;
    public const int LittleProximal = 12;
    public const int LittleIntermediate = 13;
    public const int LittleDistal = 14;
    public const int Total = 15;
  };

  public class HandLandmarksController : MonoBehaviour {
    [Tooltip("Set it to the character's animator game object.")]
    public Animator anim;
    [Tooltip("Left hand or right hand.")]
    public HandType handType;
    public int ScreenWidth;
    public int ScreenHeight;
    public NormalizedLandmarkList HandLandmarkList { private get; set; }

    // Total number of landmarks in HandPose model, per hand.
    private const int _landmarksNum = 21;
    // The thumb length of the model. To apply to a new model this value should be tweak.
    private const float _modelThumbLength = 0.02f;
    // The hand root to keep track of the position and rotation.
    private Transform _target;

    private GameObject[] _handLandmarks = new GameObject[_landmarksNum];
    private float _screenRatio = 1.0f;
    private KalmanFilter[] _kalmanFilters = new KalmanFilter[_landmarksNum];
    // The forward direction of the wrist. Computed by OrthoNormalize of the positions of index
    // finger, middle finger and wrist.
    private Vector3 _forwardVector;
    private Quaternion _wristRotation = Quaternion.Euler(0, 0, 0);
    private Transform[] _fingerTargets = new Transform[HandLandmarks.Total];

    void Start() {
      // Note: HandPose use camera perspective to determine left and right hand, which is mirrored
      // from the animator's perspective.
      var bone =
          (handType == HandType.LeftHand) ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand;
      _target = anim.GetBoneTransform(bone);

      for (int i = 0; i < _landmarksNum; i++) {
        _handLandmarks[i] = new GameObject($"HandLandmark{i}");
        _handLandmarks[i].transform.parent = transform;
        _kalmanFilters[i] = new KalmanFilter(0.125f, 1f);
      }
      _screenRatio = 1.0f * ScreenWidth / ScreenHeight;
      assignFingerTargets();
    }

    void Update() {
      if (HandLandmarkList != null) {
        NormalizedLandmark landmark0 = HandLandmarkList.Landmark[0];
        NormalizedLandmark landmark1 = HandLandmarkList.Landmark[1];
        var s = _modelThumbLength / (ToVector(landmark1) - ToVector(landmark0)).magnitude;
        var scale = new Vector3(s * _screenRatio, s, s * _screenRatio);
        for (int i = 1; i < HandLandmarkList.Landmark.Count; i++) {
          NormalizedLandmark landmark = HandLandmarkList.Landmark[i];
          Vector3 tip = Vector3.Scale(ToVector(landmark) - ToVector(landmark0), scale);
          _handLandmarks[i].transform.localPosition = _kalmanFilters[i].Update(tip);
        }
      }

      ComputeWristRotation();
      transform.position = _target.transform.position;
      _target.rotation = _wristRotation;

      ComputeFingerRotation();
    }

    void OnDrawGizmos() {
      if (_target != null) {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_target.position, 0.005f);
        Gizmos.color = Color.yellow;
        Vector3 direction = _target.TransformDirection(Vector3.forward) * 0.1f;
        if (handType == HandType.LeftHand) {
          direction = -direction;
        }
        Gizmos.DrawRay(_target.position, direction);
      }
      // DrawGizmoFingerJointsSpheres();
      // DrawGizmoFingerSkeleton();

      DrawGizmoFingerJointsForward();
    }

    private void DrawGizmoFingerJointsSpheres() {
      Gizmos.color = Color.red;
      foreach (var handLandmark in _handLandmarks) {
        if (handLandmark != null)
          Gizmos.DrawSphere(handLandmark.transform.position, 0.005f);
      }
    }

    private void DrawGizmoFingerSkeleton() {
      Gizmos.color = Color.white;
      Gizmos.DrawLine(_handLandmarks[0].transform.position, _handLandmarks[1].transform.position);
      Gizmos.DrawLine(_handLandmarks[1].transform.position, _handLandmarks[2].transform.position);
      Gizmos.DrawLine(_handLandmarks[2].transform.position, _handLandmarks[3].transform.position);
      Gizmos.DrawLine(_handLandmarks[3].transform.position, _handLandmarks[4].transform.position);
      Gizmos.DrawLine(_handLandmarks[0].transform.position, _handLandmarks[5].transform.position);
      Gizmos.DrawLine(_handLandmarks[5].transform.position, _handLandmarks[6].transform.position);
      Gizmos.DrawLine(_handLandmarks[6].transform.position, _handLandmarks[7].transform.position);
      Gizmos.DrawLine(_handLandmarks[7].transform.position, _handLandmarks[8].transform.position);
    }

    private void DrawGizmoFingerJointsForward() {
      if (_fingerTargets != null) {
        for (int i = 9; i < 12; i++) {
          var bone = _fingerTargets[i];
          if (bone != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(bone.position, bone.TransformDirection(Vector3.forward) * 0.1f);
          }
        }
      }
    }

    private Vector3 ToVector(NormalizedLandmark landmark) {
      return new Vector3(landmark.X, landmark.Y, landmark.Z);
    }

    private void ComputeWristRotation() {
      var wristTransform = transform;
      var indexFinger = _handLandmarks[5].transform.position;
      var middleFinger = _handLandmarks[9].transform.position;

      var vectorToMiddle = middleFinger - wristTransform.position;
      var vectorToIndex = indexFinger - wristTransform.position;
      Vector3.OrthoNormalize(ref vectorToMiddle, ref vectorToIndex);
      _forwardVector = Vector3.Cross(vectorToIndex, vectorToMiddle);
      _wristRotation = Quaternion.LookRotation(_forwardVector, vectorToIndex);
    }

    private void ComputeFingerRotation() {
      var rotationTable = new(int fingerId, int landmarkId1, int landmarkId2)[] {
        (0, 1, 2),    (1, 2, 3),    (2, 3, 4),    (3, 5, 6),    (4, 6, 7),
        (5, 7, 8),    (6, 9, 10),   (7, 10, 11),  (8, 11, 12),  (9, 13, 14),
        (10, 14, 15), (11, 15, 16), (12, 17, 18), (13, 18, 19), (14, 19, 20)
      };
      foreach (var (fingerId, landmarkId1, landmarkId2) in rotationTable) {
        Vector3 fingerDirection = _handLandmarks[landmarkId2].transform.position -
                                  _handLandmarks[landmarkId1].transform.position;
        Vector3 right =
            GetNormal(transform.position, _handLandmarks[landmarkId1].transform.position,
                      _handLandmarks[landmarkId2].transform.position);
        Vector3 forward = Vector3.Cross(right, fingerDirection);
        _fingerTargets[fingerId].rotation = Quaternion.LookRotation(forward, right);
      }
    }

    private void assignFingerTargets() {
      // The output of HandPose detection is mirrored here.
      if (handType == HandType.LeftHand) {
        _fingerTargets[HandLandmarks.ThumbProximal] =
            anim.GetBoneTransform(HumanBodyBones.RightThumbProximal);
        _fingerTargets[HandLandmarks.ThumbIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
        _fingerTargets[HandLandmarks.ThumbDistal] =
            anim.GetBoneTransform(HumanBodyBones.RightThumbDistal);
        _fingerTargets[HandLandmarks.IndexProximal] =
            anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);
        _fingerTargets[HandLandmarks.IndexIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
        _fingerTargets[HandLandmarks.IndexDistal] =
            anim.GetBoneTransform(HumanBodyBones.RightIndexDistal);
        _fingerTargets[HandLandmarks.MiddleProximal] =
            anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        _fingerTargets[HandLandmarks.MiddleIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
        _fingerTargets[HandLandmarks.MiddleDistal] =
            anim.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
        _fingerTargets[HandLandmarks.RingProximal] =
            anim.GetBoneTransform(HumanBodyBones.RightRingProximal);
        _fingerTargets[HandLandmarks.RingIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
        _fingerTargets[HandLandmarks.RingDistal] =
            anim.GetBoneTransform(HumanBodyBones.RightRingDistal);
        _fingerTargets[HandLandmarks.LittleProximal] =
            anim.GetBoneTransform(HumanBodyBones.RightLittleProximal);
        _fingerTargets[HandLandmarks.LittleIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
        _fingerTargets[HandLandmarks.LittleDistal] =
            anim.GetBoneTransform(HumanBodyBones.RightLittleDistal);
      } else {
        _fingerTargets[HandLandmarks.ThumbProximal] =
            anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
        _fingerTargets[HandLandmarks.ThumbIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
        _fingerTargets[HandLandmarks.ThumbDistal] =
            anim.GetBoneTransform(HumanBodyBones.LeftThumbDistal);
        _fingerTargets[HandLandmarks.IndexProximal] =
            anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
        _fingerTargets[HandLandmarks.IndexIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
        _fingerTargets[HandLandmarks.IndexDistal] =
            anim.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
        _fingerTargets[HandLandmarks.MiddleProximal] =
            anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        _fingerTargets[HandLandmarks.MiddleIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
        _fingerTargets[HandLandmarks.MiddleDistal] =
            anim.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
        _fingerTargets[HandLandmarks.RingProximal] =
            anim.GetBoneTransform(HumanBodyBones.LeftRingProximal);
        _fingerTargets[HandLandmarks.RingIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
        _fingerTargets[HandLandmarks.RingDistal] =
            anim.GetBoneTransform(HumanBodyBones.LeftRingDistal);
        _fingerTargets[HandLandmarks.LittleProximal] =
            anim.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
        _fingerTargets[HandLandmarks.LittleIntermediate] =
            anim.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
        _fingerTargets[HandLandmarks.LittleDistal] =
            anim.GetBoneTransform(HumanBodyBones.LeftLittleDistal);
      }
    }

    // <summary>
    // Get the normal to a triangle from the three corner points, a, b and c.
    // See https://docs.unity3d.com/ScriptReference/Vector3.Cross.html.
    // </summary>
    // <exception cref="Assertions.AssertionException">
    // An assertion is thrown if the sides from input vectors are parallel.
    // </exception>
    private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c) {
      // Find vectors corresponding to two of the sides of the triangle.
      Vector3 side1 = b - a;
      Vector3 side2 = c - a;

      // Cross the vectors to get a perpendicular vector, then normalize it.
      Vector3 result = Vector3.Cross(side1, side2).normalized;
      Assert.AreNotEqual(Vector3.zero, result, "The sides from input vectors are parallel.");
      return result;
    }
  }
}
