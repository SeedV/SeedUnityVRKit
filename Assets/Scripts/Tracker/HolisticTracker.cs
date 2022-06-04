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
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.Holistic;
using UnityEngine;
using UnityEngine.Events;

namespace SeedUnityVRKit {
  [RequireComponent(typeof(HolisticTrackingGraph))]
  public class HolisticTracker : BaseTracker {
    public UnityEvent<NormalizedLandmarkList> FaceLandmarksOutputEvent;
    public UnityEvent<NormalizedLandmarkList> PoseLandmarksOutputEvent;
    public UnityEvent<NormalizedLandmarkList> LeftHandLandmarksOutputEvent;
    public UnityEvent<NormalizedLandmarkList> RightHandLandmarksOutputEvent;

    private List<NormalizedLandmarkList> _faceLandmarks = new List<NormalizedLandmarkList>();
    private List<NormalizedLandmarkList> _poseLandmarks = new List<NormalizedLandmarkList>();
    private List<NormalizedLandmarkList> _leftHandLandmarks = new List<NormalizedLandmarkList>();
    private List<NormalizedLandmarkList> _rightHandLandmarks = new List<NormalizedLandmarkList>();

    public void Update() {
      if (_faceLandmarks.Count > 0) {
        lock (_faceLandmarks) {
          foreach (NormalizedLandmarkList landmark in _faceLandmarks) {
            FaceLandmarksOutputEvent.Invoke(landmark);
          }
          _faceLandmarks.Clear();
        }
      }
      if (_poseLandmarks.Count > 0) {
        lock (_poseLandmarks) {
          foreach (NormalizedLandmarkList landmark in _poseLandmarks) {
            PoseLandmarksOutputEvent.Invoke(landmark);
          }
          _poseLandmarks.Clear();
        }
      }
      if (_leftHandLandmarks.Count > 0) {
        lock (_leftHandLandmarks) {
          foreach (NormalizedLandmarkList landmark in _leftHandLandmarks) {
            LeftHandLandmarksOutputEvent.Invoke(landmark);
          }
          _leftHandLandmarks.Clear();
        }
      }
      if (_rightHandLandmarks.Count > 0) {
        lock (_rightHandLandmarks) {
          foreach (NormalizedLandmarkList landmark in _rightHandLandmarks) {
            RightHandLandmarksOutputEvent.Invoke(landmark);
          }
          _rightHandLandmarks.Clear();
        }
      }
    }

    public override void AddEventHandler() {
      _graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
      _graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
      _graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
      _graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
    }

    private void OnFaceLandmarksOutput(object stream,
                                       OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      lock (_faceLandmarks) {
        _faceLandmarks.Add(eventArgs.value);
      }
    }

    private void OnPoseLandmarksOutput(object stream,
                                       OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      lock (_poseLandmarks) {
        _poseLandmarks.Add(eventArgs.value);
      }
    }

    private void OnLeftHandLandmarksOutput(object stream,
                                           OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      lock (_leftHandLandmarks) {
        _leftHandLandmarks.Add(eventArgs.value);
      }
    }

    private void OnRightHandLandmarksOutput(object stream,
                                            OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      lock (_rightHandLandmarks) {
        _rightHandLandmarks.Add(eventArgs.value);
      }
    }
  }
}
