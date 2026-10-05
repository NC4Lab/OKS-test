# OKS Test Scene (Unity)
 
A simple Unity scene for testing a roll optokinetic stimulus (OKS) at different frequencies and amplitudes. It was built as a practice and test tool for the OKS component of a larger multisensory self-orientation paradigm (NC4 lab), mainly to check whether corner frequencies derived from published SVV time constants give a perceptually reasonable stimulus.
 
This is a test build, not an experiment. No data are collected.
 
## What it does
 
- Scatters 2000 white dots on a sphere around the camera, using a seeded random layout so the dot field is identical on every run.
- Rolls the whole field sinusoidally about the line of sight (world Z axis) at an adjustable frequency and amplitude.
- Keeps the camera at the origin on a black background, with a faint fixation crosshair at screen center.
- Lets you click and drag to look around (yaw and pitch only; roll is locked, so any roll you see comes only from the OKS field).
## Controls
 
| Input | Action |
|---|---|
| Left mouse drag | Look around |
| R | Reset view |
| 1 to 5 | Frequency presets: 0.02, 0.04, 0.1, 0.2, 0.5 Hz |
| Tab | Show / hide the control panel |
| On-screen sliders | Frequency (log scale, 0.01 to 0.5 Hz) and amplitude (2 to 30 degrees) |
 
## Default stimulus parameters
 
| Parameter | Value |
|---|---|
| Motion | Sinusoidal roll: angle(t) = A sin(2 pi f t) |
| Frequency | 0.02 Hz (range 0.01 to 0.5 Hz) |
| Amplitude | 30 degrees peak |
| Dots | 2000 on a sphere of radius 10; diameters 0.1 to 0.2 units (about 0.55 to 1.1 degrees at the camera); seed 12345 |
| Camera | At origin, looking down +Z, 60 degree vertical FOV |
 
Peak roll speed is 2 pi f A, for example about 3.8 deg/s at 0.02 Hz and about 7.5 deg/s at 0.04 Hz with 30 degree amplitude.
 
## Corner frequencies
 
Time constants of the SVV exponential responses are taken from Niehof et al., doi: 10.1152/jn.00083.2019. OKS about 7 s and GVS about 4 s. Corner frequency is computed as f_c = 1 / (2 pi tau):
 
- OKS: about 0.023 Hz (rounded to 0.02 Hz in the presets)
- GVS: about 0.040 Hz (rounded to 0.04 Hz in the presets)
The 30 degree amplitude follows Niehof et al. (2019).
