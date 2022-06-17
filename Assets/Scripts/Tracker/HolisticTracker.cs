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
    public UpperBodyAnimator Animator;
    [SerializeField]
    private HolisticLandmarkListAnnotationController _holisticAnnotationController;
    public HandLandmarksController LeftHandLandmarksController;
    public HandLandmarksController RightHandLandmarksController;

    public void Update() {
      systemStats?.IncrementFrameRendered();
    }

    public override void AddEventHandler() {
      OnFaceLandmarksOutput += Animator.OnFaceLandmarksOutput;
      OnPoseLandmarksOutput += Animator.OnPoseLandmarksOutput;
      OnFaceLandmarksOutput += _holisticAnnotationController.DrawFaceLandmarkListLater;
      OnPoseLandmarksOutput += _holisticAnnotationController.DrawPoseLandmarkListLater;
      OnLeftHandLandmarksOutput += _holisticAnnotationController.DrawLeftHandLandmarkListLater;
      OnRightHandLandmarksOutput += _holisticAnnotationController.DrawRightHandLandmarkListLater;
      // OnLeftHandLandmarksOutput += LeftHandLandmarksController.OnHandLandmarksOutput;
      // OnRightHandLandmarksOutput += RightHandLandmarksController.OnHandLandmarksOutput;
    }
  }
}
