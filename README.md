# tak-lite

This is an experiment in .Net Maui to create a cross platform TAK (Team Awareness Kit) client application.  A large part of this has been reverse engineering the TAK message protocols.

The main objective is to provide consistent GIS Mapping features cross platform, the existing Department of Defense applications have inconsistent support for simple capabilities like Keyhole Markup Language and overlays.  The featureset is paired down to a minimum set of features needed  by part time users of the Team Awareness kit functionality. Think of this as part time Emergency Responders or other volunteers that would all bring their own devices to help with an emergency.

Added basic user tracking and updating from multiple TAK servers
Added capabilities to connect the Alpha Ares application servers, the protocol was reverse engineered.
  - Login
  - Join existing Game, including qr code scaning.
Added certificate enrollment to TAK servers.
Added ability to load a KML file objects and image overlays.
Added ability to configure callsign, team (color), role, and subteam for filtering.
Added ability to scan a QR code to download a TAK connection file, then load it automatically.




