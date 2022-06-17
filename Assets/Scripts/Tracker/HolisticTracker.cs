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

namespace SeedUnityVRKit
{
    [RequireComponent(typeof(HolisticTrackingGraph))]
    public class HolisticTracker : BaseTracker
    {

        [SerializeField]
        private UpperBodyAnimator _modelAnimator;
        [SerializeField]
        private HandLandmarksController _leftHandAnimator;
        [SerializeField]
        private HandLandmarksController _rightHandAnimator;

        public override void AddEventHandler()
        {
            _graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
            _graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
            _graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
            _graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;

        }

        private void OnPoseLandmarksOutput(object stream,
                                             OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            NormalizedLandmarkList poseLandmarks = eventArgs.value;
            if (_modelAnimator != null && poseLandmarks != null)
            {
                _modelAnimator.OnPoseLandmarksOutput(poseLandmarks);
            }
        }

        private void OnFaceLandmarksOutput(object stream,
                                           OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            NormalizedLandmarkList faceLandmarks = eventArgs.value;
            if (_modelAnimator != null && faceLandmarks != null)
            {
                _modelAnimator.OnFaceLandmarksOutput(faceLandmarks);
            }
        }
        private void OnLeftHandLandmarksOutput(object stream,
                                           OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            NormalizedLandmarkList leftHandLandmarks = eventArgs.value;
            if (_leftHandAnimator != null && leftHandLandmarks != null)
            {
                _leftHandAnimator.OnHandLandmarksOutput(leftHandLandmarks);
            }
        }
        private void OnRightHandLandmarksOutput(object stream,
                                           OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            NormalizedLandmarkList rightHandLandmarks = eventArgs.value;
            if (_leftHandAnimator != null && rightHandLandmarks != null)
            {
                _rightHandAnimator.OnHandLandmarksOutput(rightHandLandmarks);
            }
        }
    }
}
