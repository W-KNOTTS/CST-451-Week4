**Slim Media Player**

*William Knotts*

*Grand Canyon University: College of Science, Engineering, & Technology*

*CST-452: Senior Project II*

*Professor Amr Elchouemi*

*Revision: 5*

*May 19, 2024*

**Project Summary:**

This project was created to replace my older media player solutions. Newer media players track users and their habits, 
creating a finger print to target their users for sales, media, and just the general value that comes with the collected 
data points to be sold to others for use. This media player has that in mind by creating a modular and clean environment 
for users to enjoy their media. The application will be built in C# WPF because of its native media playback capability 
but at the cost of being a Windows only application. UWP and JAVA were too restrictive and required stacks of 3rd party 
libraries. With WPF only three libraries are needed to read/ write the metadata of the selected file, rip music from CDs and
get online track details. C# WPF meets all the requirements of the media player app and will be used moving forward for 
this project.

![Example Image](/Resources/Capture2.PNG "Demo")

![Example Image](/Resources/Week4UML-Sitemap-FlowChart.png "Diagrams")


____________________________________
**Build instructions**

*Get Project*

    git clone https://github.com/W-KNOTTS/CST-451-Week4.git

*Open FinalProjectWPF-2.csproj*

Build the project, install any required libraries.

Play and rip your music!
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


