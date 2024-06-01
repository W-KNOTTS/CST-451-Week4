## Slim Media Player

[Link to Project Portfolio](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/portfolio.md)

[Link to compiled application](https://github.com/W-KNOTTS/CST-452-Portfolio/releases/tag/v1.0.0R)

---

## README.MD

*William Knotts*

*Grand Canyon University: College of Science, Engineering, & Technology*

*CST-452: Senior Project II*

*Professor Amr Elchouemi*

*Revision: 7*

*June 02, 2024*

**Project Summary:**

This project was created to replace my older media player solutions. Newer media players track users and their habits, 
creating a finger print to target their users for sales, media, and just the general value that comes with the collected 
data points to be sold to others for use. This media player has that in mind by creating a modular and clean environment 
for users to enjoy their media. The application will be built in C# WPF because of its native media playback capability 
but at the cost of being a Windows only application. UWP and JAVA were too restrictive and required stacks of 3rd party 
libraries. With WPF only three libraries are needed to read/ write the metadata of the selected file, rip music from CDs and
get online track details. C# WPF meets all the requirements of the media player app and will be used moving forward for 
this project.


____________________________________
**Build instructions**

*Get Project*

    git clone --branch master https://github.com/W-KNOTTS/CST-451-Week4.git

*Open FinalProjectWPF-2.csproj*

Build the project, install any required libraries.

Or Just download the precompiled program from the release page, link at the top of this page.

Rip and Play your music!

____________________________________

## Operation Instructions

**No CD in the Drive at Start**

![Program Started Without CD In the Drive](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/FirstOpen-NoCD.PNG)

Above shows an example if the application starts with no CD in the CD Drive

**CD in the Drive at Application Start**

![Program Started With CD In the Drive](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/FirstOpen-wCD.PNG)

Above shows an example if the application starts with a CD in the CD Drive

____________________________________

**Loading a CD to Rip Tracks**

![Loading a CD In the Drive](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/RefreshCD-SelectDrive.png)

To load a CD, Click the CD drive Combo Box and select your CDRom drive.

**Select a CD track to Rip and save loaction**

![Ripping a CD Track](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/RipButtonCDLoadedSaveLocation.PNG)

Select a track in the list and click the "Rip Song Button", then Create/ Select a location to save the MP3 file.

![Ripping a CD Track](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/RipSelectedTrack.PNG)

Once the rip starts you will be presented with a "Ripping progress %" below the metadata

**Selected Track Rip Finished**

![Ripping a CD Track](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/RipFinished-MetadataUpdatedPNG.PNG)

Once the track copies to your save loaction, the MP3 will have the track number inserted as the track title and the Rip % will now say "Ripping completed successfully" and you will be presented with a popup letting you know the metadata was updated on your new MP3 file.

**New CD loaded to Rip Tracks**

![Loading a CD In the Drive](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/RefreshCD-CDRefreshed.png)

If you wish to change the disc that you would like to rip, click the cd rom drive combo box to refresh the CD. When the new CD Loads, metatadata will be loaded from MusicBrainz based on the Disc ID.
____________________________________

**Audio Playback**

![To load a MP3 file"](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/SelectMP3Directory.PNG)

To load a MP3 file, click the "Select Directory Button", navigate to the directory where the MP3 files were ripped to and double click a track

![Playing MP3](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/PlayMP3File.PNG)

To play the track, select the song in the list and click play. The next track in the list will load once the current song finishes.

____________________________________

**Video Playback**

![To load a Video file"](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/SelectVideoDirectory.PNG)

To load a video file, click the "Select Directory Button", navigate to the directory where the video files saved and double click a video.

![Playing Video](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/VideoSelected-playing.PNG)

To play the video, select the video in the list and click play. The next video in the list will load once the current one finishes.

____________________________________

**Manual Metadata Updating**

![To Update Metadata"](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/MetaDataManualUpdate1.PNG)

To update the metadata of a track manually, select the track in the list and fill out the metadata you wish to change

![Playing Video](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/MetaDataManualUpdate2.PNG)

Once you enter your changes, click the "Update Metadata button", you will then be presented with a successful update window. Thats It! now it has your entered metadata.
____________________________________


***Project Planning***

![Example Image](https://github.com/W-KNOTTS/CST-451-Week4/blob/master/Resources/Capture2.PNG)

![Example Image](https://github.com/W-KNOTTS/CST-452-Portfolio/blob/main/Resources/Week4UML-Sitemap-FlowChart.png)


____________________________________

***Copyright Compliance***

**TagLibSharp**

TagLibSharp is an open-source library for reading and editing audio metadata. licensed under the GNU Lesser General Public License (LGPL) version 2.1.z

TagLibSharp. (n.d.). NuGet Gallery. https://www.nuget.org/packages/TagLibSharp#dependencies-body-tab

**CSAudioCDRipper 1.1.4**

License file

Copyright (c) Microncode.com

The CSAudioConverter is available for FREE for learning or for FREE purposes, 
for commercial or any other use please order a license.
This source is subject to the Microsoft Public License. 
See https://opensource.org/licenses/ms-pl.html.
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

Documentation: https://github.com/W-KNOTTS/CST-451-Week4/blob/master/Resources/CSAudioCDRipperDocumentation.chm

CSAudioCDRipper. (n.d.). NuGet Gallery, from https://www.nuget.org/packages/CSAudioCDRipper

**MusicBrainzAPI 3.1.0**

The MIT License (MIT)

Copyright (c) 2013 avatar29A

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions: The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOF

MusicBrainzAPI. (n.d.). NuGet Gallery, from https://www.nuget.org/packages/MusicBrainzAPI

MusicBrainzAPI. (n.d.). musicbrainz, from https://musicbrainz.org/doc/MusicBrainz_API


