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

using System;
using System.Collections;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.Holistic;
using UnityEngine;

namespace SeedUnityVRKit {
  [RequireComponent(typeof(HolisticTrackingGraph))]
  public class HolisticTracker : BaseTracker {
    [SerializeField]
    private UpperBodyAnimator _modelAnimator = null;

    [SerializeField]
    private HolisticLandmarkListAnnotationController _holisticAnnotationController;

    [SerializeField]
    private HandLandmarksController _leftHandLandmarksController;

    [SerializeField]
    private HandLandmarksController _rightHandLandmarksController;

    private event Action<NormalizedLandmarkList> _onFaceLandmarksOutputEvent;
    private event Action<NormalizedLandmarkList> _onPoseLandmarksOutputEvent;
    private event Action<NormalizedLandmarkList> _onLeftHandLandmarksOutputEvent;
    private event Action<NormalizedLandmarkList> _onRightHandLandmarksOutputEvent;

    public override void AddEventHandler() {
      _onFaceLandmarksOutputEvent += _holisticAnnotationController.DrawFaceLandmarkListLater;
      _onFaceLandmarksOutputEvent += _modelAnimator.OnFaceLandmarksOutput;
      _onPoseLandmarksOutputEvent += _holisticAnnotationController.DrawPoseLandmarkListLater;
      _onPoseLandmarksOutputEvent += _modelAnimator.OnPoseLandmarksOutput;
      _onLeftHandLandmarksOutputEvent += (landmarkList) =>
          _leftHandLandmarksController.HandLandmarkList = landmarkList;
      _onLeftHandLandmarksOutputEvent +=
          _holisticAnnotationController.DrawLeftHandLandmarkListLater;
      _onRightHandLandmarksOutputEvent += (landmarkList) =>
          _rightHandLandmarksController.HandLandmarkList = landmarkList;
      _onRightHandLandmarksOutputEvent +=
          _holisticAnnotationController.DrawRightHandLandmarkListLater;
      _graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
      _graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
      _graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
      _graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
    }

    private void OnFaceLandmarksOutput(object stream,
                                       OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      NormalizedLandmarkList landmarkList = eventArgs.value;
      if (landmarkList != null) {
        _onFaceLandmarksOutputEvent?.Invoke(landmarkList);
      }
    }

    private void OnPoseLandmarksOutput(object stream,
                                       OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      NormalizedLandmarkList landmarkList = eventArgs.value;
      if (landmarkList != null) {
        _onPoseLandmarksOutputEvent?.Invoke(landmarkList);
      }
    }

    private void OnLeftHandLandmarksOutput(object stream,
                                           OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      NormalizedLandmarkList landmarkList = eventArgs.value;
      if (landmarkList != null) {
        _onLeftHandLandmarksOutputEvent?.Invoke(landmarkList);
      }
    }

    private void OnRightHandLandmarksOutput(object stream,
                                            OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      NormalizedLandmarkList landmarkList = eventArgs.value;
      if (landmarkList != null) {
        _onRightHandLandmarksOutputEvent?.Invoke(landmarkList);
      }
    }
  }
}
