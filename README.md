# WSL2 Instance IP

## Overview

This is a small console application for Windows (x64) that is designed for use alongside WSL2 (Windows Subsystem for Linux).

It uses the `wsl.exe` command to identify the IP address that has been assigned to the WSL2 instance, and adds that IP address to your system's hosts file under the hostname `host.wsl2.internal`

## Why would I need this?

Your mileage may vary on this one, but the reason that I created this was that I am running a PHP development environment consisting of:

* Windows 10 Pro
* WSL2 with Ubuntu 20.04 (LTS)
* Docker Desktop for Windows
* Various Docker containers, including one running PHP and Xdebug 3
* JetBrains PhpStorm 2021.3, running inside the WSL2 Ubuntu distro (so, the Linux version of PhpStorm, not the Windows version)

The issue I was having was that Xdebug was unable to connect to the PhpStorm debug client as none of the usual hostnames (e.g. `host.docker.internal`) worked - these all relate to the Windows side, NOT the Ubuntu VM running as part of WSL2.

I found that I could get the correct IP address to connect to by running `wsl.exe hostname -I` and then set my `xdebug.client_host` setting to that IP address. This worked great, but unfortunately this IP address changes every single time the computer is restarted, so I needed someway of automating this.

Now that I've written this application, I just point `xdebug.client_host` to `host.wsl2.internal` and now all is well in the world!

## Inspiration

This application is based on a similar application found here: https://github.com/silverfoxy/wsl2_host_ip

I tried this on my machine and although it did what it said on the tin, unfortunately the IP address it found wasn't the correct one (don't ask me why!) so I've essentially rewritten this application to use the IP address from `wsl.exe hostname -I` instead of detecting it from the network interfaces directly.

## Installation

Download the latest release build, and copy the files inside to a suitable location on your PC, for example `C:\Program Files\WSL2_Instance_IP`.

Create a scheduled task (using Task Scheduler) that triggers whenever you logon to the PC, and runs `wsl2instanceip.exe`. You'll need to edit the task after creation, making sure that you tick the box to run the program using the highest privileges found near the bottom of the General tab.

Now, whenever you logon, an entry should be updated in the hosts file (e.g. `C:\Windows\System32\drivers\etc\hosts`) containing the IP address and pointing `host.wsl2.internal` at it.

## Limitations

My understanding of WSL2 is that it's entirely possible for `wsl.exe hostname -I` to return multiple IP addresses - I'm not sure what causes this, presumably having multiple distros installed on WSL2 or something like that, but if this applies to you then this tool will not be of any use to you.

If you run `wsl.exe hostname -I` from a command prompt and see more than one IP address, then this won't work for you I'm afraid.

Also note that this application is provided as-is, you're welcome to review the source code to make sure you're happy to run it, along with downloading the code and building an executable from it yourself if needed.

I don't take any responsibility if it buggers anything up on your system though, you run the program entirely at your own risk.