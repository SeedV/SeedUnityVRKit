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
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.Holistic;
using UnityEngine;

[RequireComponent(typeof(HolisticTrackingGraph))]
public class HolisticTracker : BaseTracker {
  [SerializeField] private UpperBodyAnimator _modelAnimator = null;

  [SerializeField] private HolisticLandmarkListAnnotationController _holisticAnnotationController;
  private HolisticTrackingGraph _graphRunner;

  public override IEnumerator Start() {
    _graphRunner = GetComponent<HolisticTrackingGraph>();
    return base.Start();
  }

  public override WaitForResult CreateGraphInitRequest() {
    return _graphRunner.WaitForInit(RunningMode.Async);
  }

  public override void StartGraph(int rotation, bool hFlip, bool vFlip) {
    SidePacket sidePacket = _graphRunner.BuildSidePacket(rotation, hFlip, vFlip);
    _graphRunner.StartRun(sidePacket);
  }

  public override void AddEventHandler() {
    _graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
    _graphRunner.OnFaceLandmarksOutput += _modelAnimator.OnFaceLandmarksOutput;
    _graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
    _graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
    _graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
  }

  public override void ProcessTextureFrame(TextureFrame textureFrame) {
    _graphRunner.AddTextureFrameToInputStream(textureFrame);
  }

  private void OnFaceLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs) {
    _holisticAnnotationController.DrawFaceLandmarkListLater(eventArgs.value);
  }

  private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs) {
    _holisticAnnotationController.DrawPoseLandmarkListLater(eventArgs.value);
  }

  private void OnLeftHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs) {
    _holisticAnnotationController.DrawLeftHandLandmarkListLater(eventArgs.value);
  }

  private void OnRightHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs) {
    _holisticAnnotationController.DrawRightHandLandmarkListLater(eventArgs.value);
  }
}