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

    private NormalizedLandmarkOutputSink _faceLandmarkOutputSink =
        new NormalizedLandmarkOutputSink();
    private NormalizedLandmarkOutputSink _poseLandmarkOutputSink =
        new NormalizedLandmarkOutputSink();
    private NormalizedLandmarkOutputSink _leftHandLandmarkOutputSink =
        new NormalizedLandmarkOutputSink();
    private NormalizedLandmarkOutputSink _rightHandLandmarkOutputSink =
        new NormalizedLandmarkOutputSink();

    public void Update() {
      systemStats?.IncrementFrameRendered();
      _faceLandmarkOutputSink.Consume(e => FaceLandmarksOutputEvent.Invoke(e));
      _poseLandmarkOutputSink.Consume(e => PoseLandmarksOutputEvent.Invoke(e));
      _leftHandLandmarkOutputSink.Consume(e => LeftHandLandmarksOutputEvent.Invoke(e));
      _rightHandLandmarkOutputSink.Consume(e => RightHandLandmarksOutputEvent.Invoke(e));
    }

    public override void AddEventHandler() {
      _graphRunner.OnFaceLandmarksOutput += (stream, eventArgs) => {
        _faceLandmarkOutputSink.Add(eventArgs.value);
        systemStats?.IncrementFrameProcessed();
      };
      _graphRunner.OnPoseLandmarksOutput += (stream, eventArgs) =>
          _poseLandmarkOutputSink.Add(eventArgs.value);
      _graphRunner.OnLeftHandLandmarksOutput += (stream, eventArgs) =>
          _leftHandLandmarkOutputSink.Add(eventArgs.value);
      _graphRunner.OnRightHandLandmarksOutput += (stream, eventArgs) =>
          _rightHandLandmarkOutputSink.Add(eventArgs.value);
    }

    class NormalizedLandmarkOutputSink {
      private List<NormalizedLandmarkList> _landmarkQueue = new List<NormalizedLandmarkList>();
      public void Add(NormalizedLandmarkList landmark) {
        lock (_landmarkQueue) {
          _landmarkQueue.Add(landmark);
        }
      }
      public void Consume(Action<NormalizedLandmarkList> invoke) {
        if (_landmarkQueue.Count > 0) {
          lock (_landmarkQueue) {
            foreach (NormalizedLandmarkList e in _landmarkQueue) {
              invoke(e);
            }
          }
          _landmarkQueue.Clear();
        }
      }
    }
  }
}
