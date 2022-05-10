#include <stdio.h>

#include <opencv2/calib3d.hpp>

extern "C" {

void solvePnP(float width, float height,
              float* objectPointsArray, float* imagePointsArray, float* cameraMatrixArray,
              float* distCoeffsArray, float* rvecArray, float* tvecArray,
              bool useExtrinsicGuess) {
    cv::Mat objectPoints(468, 3, CV_32F, objectPointsArray);
    cv::Mat imagePoints(468, 2, CV_32F, imagePointsArray);
    cv::Mat cameraMatrix(3, 3, CV_32F, new float[] {
        width, 0, width / 2,
        0, width, height / 2,
        0, 0, 1
    });
    cv::Mat distCoeffs(4, 1, CV_32F, cv::Scalar::all(0));
    cv::Mat rvec(3, 1, CV_32F, rvecArray);
    cv::Mat tvec(3, 1, CV_32F, tvecArray);
    cv::solvePnP(objectPoints, imagePoints, cameraMatrix, distCoeffs, rvec, tvec, useExtrinsicGuess);
}

}
