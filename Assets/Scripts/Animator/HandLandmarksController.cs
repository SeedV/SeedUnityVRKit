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
using Color = UnityEngine.Color;

using Mediapipe;
using Mediapipe.Unity;

namespace SeedUnityVRKit {
  public enum HandType { LeftHand, RightHand }
  public class HandLandmarksController : MonoBehaviour {
    [Tooltip("Set it to the character's animator game object.")]
    public Animator anim;
    [Tooltip("Left hand or right hand.")]
    public HandType handType;
    public int ScreenWidth;
    public int ScreenHeight;

    // Total number of landmarks in HandPose model, per hand.
    private const int _landmarksNum = 21;
    // The thumb length of the model. To apply to a new model this value should be tweak.
    private const float _modelThumbLength = 0.02f;
    // The hand root to keep track of the position and rotation.
    private Transform _target;

    private GameObject[] _handLandmarks = new GameObject[_landmarksNum];
    private float _screenRatio = 1.0f;

    void Start() {
      // Note: HandPose use camera perspective to determine left and right hand, which is mirrored
      // from the animator's perspective.
      var bone =
          (handType == HandType.LeftHand) ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand;
      _target = anim.GetBoneTransform(bone);

      for (int i = 0; i < _landmarksNum; i++) {
        _handLandmarks[i] = new GameObject($"HandLandmark{i}");
        _handLandmarks[i].transform.parent = transform;
      }
      _screenRatio = 1.0f * ScreenWidth / ScreenHeight;
    }

    void Update() {
      transform.position = _target.transform.position;
      _target.rotation = ComputeWristRotation();
    }

    void OnDrawGizmos() {
      Gizmos.color = Color.red;
      if (_target != null) {
        Gizmos.DrawSphere(_target.position, 0.005f);
      }
      foreach (var handLandmark in _handLandmarks) {
        if (handLandmark != null)
          Gizmos.DrawSphere(handLandmark.transform.position, 0.005f);
      }
    }

    public void OnHandLandmarksOutput(NormalizedLandmarkList landmarkList) {
      if (landmarkList != null) {
        NormalizedLandmark landmark0 = landmarkList.Landmark[0];
        NormalizedLandmark landmark1 = landmarkList.Landmark[1];
        var s = _modelThumbLength / (ToVector(landmark1) - ToVector(landmark0)).magnitude;
        var scale = new Vector3(s * _screenRatio, s, s * _screenRatio);
        for (int i = 1; i < landmarkList.Landmark.Count; i++) {
          NormalizedLandmark landmark = landmarkList.Landmark[i];
          Vector3 tip = Vector3.Scale(ToVector(landmark) - ToVector(landmark0), scale);
          _handLandmarks[i].transform.localPosition = tip;
        }
      }
    }

    private Vector3 ToVector(NormalizedLandmark landmark) {
      return new Vector3(landmark.X, landmark.Y, landmark.Z);
    }

    private Quaternion ComputeWristRotation() {
      var wristTransform = transform;
      var indexFinger = _handLandmarks[5].transform.position;
      var middleFinger = _handLandmarks[9].transform.position;

      var vectorToMiddle = middleFinger - wristTransform.position;
      var vectorToIndex = indexFinger - wristTransform.position;
      Vector3.OrthoNormalize(ref vectorToMiddle, ref vectorToIndex);
      Vector3 normalVector = Vector3.Cross(vectorToIndex, vectorToMiddle);
      return Quaternion.LookRotation(normalVector, vectorToIndex);
    }
  }
}
