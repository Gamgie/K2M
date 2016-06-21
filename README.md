# K2M
It is a Kinect OSC streamer to transmit joint, bones, body information.

# OSC messages send
/k2m/body/entered <body.trackingID> : a new body is detected
/k2m/body/left <body.trackingID> : a body left kinect field of view
/k2m/body/update <body.trackingID> <LeftHandState> <RightHandState> <sendExtendedSkeleton> 
/k2m/joints <x> <y> <z> 
