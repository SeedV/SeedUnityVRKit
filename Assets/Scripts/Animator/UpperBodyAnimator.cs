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

using Mediapipe;
using Mediapipe.Unity;
using UnityEngine;

namespace SeedUnityVRKit {
  // <summary>An animator to visualize upper body and face.</summary>
  public class UpperBodyAnimator : MonoBehaviour {
    [Tooltip("Max rotation angle in degree.")]
    [Range(0, 45f)]
    public float MaxRotationThreshold = 40f;
    [Tooltip("Screen width used as to scale the recognized normalized landmarks.")]
    public float ScreenWidth = 1920;
    [Tooltip("Screen height used as to scale the recognized normalized landmarks.")]
    public float ScreenHeight = 1080;
    private const string MthDefConst = "MTH_DEF";
    private const string EyeDefConst = "EYE_DEF";
    private const string ElDefConst = "EL_DEF";
    private static readonly Quaternion _neckInitRotation = Quaternion.Euler(0, -90, -90);
    /// <summary>The neck joint to control head rotation.</summary>
    private Transform _neck;
    /// <summary>Face landmark recognizer.</summary>
    private FaceLandmarksRecognizer _faceLandmarksRecognizer;
    private NormalizedLandmarkList _faceLandmarkList;
    /// <summary>Pose landmark recognizer.</summary>
    private PoseLandmarksRecognizer _poseLandmarksRecognizer;
    private NormalizedLandmarkList _poseLandmarkList;
    private SkinnedMeshRenderer _mouthMeshRenderer;
    private SkinnedMeshRenderer _eyeMeshRenderer;
    private SkinnedMeshRenderer _elMeshRenderer;

    private Joint[] _joints = new Joint[Landmarks.Total];

    void Start() {
      var anim = GetComponent<Animator>();

      setupJoints(anim);
      _neck = anim.GetBoneTransform(HumanBodyBones.Neck);
      _faceLandmarksRecognizer = new FaceLandmarksRecognizer(ScreenWidth, ScreenHeight);
      _poseLandmarksRecognizer = new PoseLandmarksRecognizer(ScreenWidth, ScreenHeight);
      _mouthMeshRenderer = GameObject.Find(MthDefConst).GetComponent<SkinnedMeshRenderer>();
      _eyeMeshRenderer = GameObject.Find(EyeDefConst).GetComponent<SkinnedMeshRenderer>();
      _elMeshRenderer = GameObject.Find(ElDefConst).GetComponent<SkinnedMeshRenderer>();
    }

    private void setupJoints(Animator anim) {
      // Right Arm
      _joints[Landmarks.RightShoulder] =
          new Joint(anim.GetBoneTransform(HumanBodyBones.RightUpperArm));
      _joints[Landmarks.RightElbow] =
          new Joint(anim.GetBoneTransform(HumanBodyBones.RightLowerArm));
      _joints[Landmarks.RightWrist] = new Joint(anim.GetBoneTransform(HumanBodyBones.RightHand));

      // Left Arm
      _joints[Landmarks.LeftShoulder] =
          new Joint(anim.GetBoneTransform(HumanBodyBones.LeftUpperArm));
      _joints[Landmarks.LeftElbow] = new Joint(anim.GetBoneTransform(HumanBodyBones.LeftLowerArm));
      _joints[Landmarks.LeftWrist] = new Joint(anim.GetBoneTransform(HumanBodyBones.LeftHand));

      // Hip
      _joints[Landmarks.Hip] = new Joint(anim.GetBoneTransform(HumanBodyBones.Hips));

      // Connections
      // Right Arm
      _joints[Landmarks.RightShoulder].Child = _joints[Landmarks.RightElbow];
      _joints[Landmarks.RightElbow].Child = _joints[Landmarks.RightWrist];
      _joints[Landmarks.RightElbow].Parent = _joints[Landmarks.RightShoulder];

      // Left Arm
      _joints[Landmarks.LeftShoulder].Child = _joints[Landmarks.LeftElbow];
      _joints[Landmarks.LeftElbow].Child = _joints[Landmarks.LeftWrist];
      _joints[Landmarks.LeftElbow].Parent = _joints[Landmarks.LeftShoulder];

      // Assuming body is always facing the -Z axis.
      // In the future if we need to turn the upper body, we may revise this.
      Vector3 forward = new Vector3(0, 0, -1);
      foreach (Joint joint in _joints) {
        if (joint != null && joint.Child != null) {
          joint.Forward = Quaternion.LookRotation(joint.position - joint.Child.position, forward);
        }
      }
      Joint hip = _joints[Landmarks.Hip];
      hip.Forward = Quaternion.LookRotation(forward);
    }

    void LateUpdate() {
      if (_faceLandmarkList != null) {
        FaceLandmarks faceLandmarks = _faceLandmarksRecognizer.recognize(_faceLandmarkList);
        _neck.rotation = faceLandmarks.FaceRotation * _neckInitRotation;
        SetMouth(faceLandmarks.MouthAspectRatio);
        SetEye(faceLandmarks.LeftEyeShape == EyeShape.Close &&
               faceLandmarks.RightEyeShape == EyeShape.Close);
      }

      if (_poseLandmarkList != null) {
        foreach (PoseLandmark poseLandmark in _poseLandmarksRecognizer.recognize(
                     _poseLandmarkList)) {
          _joints[poseLandmark.Id].SetRotation(poseLandmark.Rotation);
        }
      }
    }

    private void SetMouth(float ratio) {
      _mouthMeshRenderer.SetBlendShapeWeight(2, ratio * 100);
    }

    private void SetEye(bool close) {
      if (close) {
        _eyeMeshRenderer.SetBlendShapeWeight(6, 100);
        _elMeshRenderer.SetBlendShapeWeight(6, 100);
      } else {
        _eyeMeshRenderer.SetBlendShapeWeight(6, 0);
        _elMeshRenderer.SetBlendShapeWeight(6, 0);
      }
    }

    public void OnFaceLandmarksOutput(NormalizedLandmarkList list) {
      _faceLandmarkList = list;
    }

    public void OnPoseLandmarksOutput(NormalizedLandmarkList list) {
      _poseLandmarkList = list;
    }
  }
}
