# <h6>RAKandACOVirtualStation_OCC</h6>
* Permission obtained from RAK & ACO General and Allied Services, Inc.
  * Source files shall be available to the the software maintainer should virtual machines be added or modified; or as hypervisor (tested on VirtualBox 4.2.20 - released Nov 28th 2013) is upgraded that might introduce breaking changes.
  * Virtual machines' images shall not be available; payroll systems are only accessible within LAN. Software maintainer must create his/her own instance of Virtualmachines for testing.
  * Contact me [mvmagbero@gmail.com] for additional questions as to  how to get started on developer environment and required dependencies to run this software and interface with Virtualbox web service.
  

#<h6>Requirements<h6>
1. Tested on Virtualbox 4.2.20. 
  * NOTE: Later versions introduce breaking changes. SOAP proxy library (WDSL) should be generated against the newer version).
  * Remote access is NOT handled by guest OS, but handled by VirtualBox VRDP protocol. Shared Desktop screen instances is one of the requirements of this software/project (i.e. Payroll Supervisor remotely checks staff's report prior printing and final submission). Unfortunately, Win7,Win8/8.1 guess OSes does NOT allow same Window desktop screen, simultaneous access (Teamviewer behavior) without nasty workarounds and registry hacks.
  * MUST Use RDP 8.0 and above for smooth streaming performance, on both Host and client consumer. see [https://support.microsoft.com/en-us/kb/2592687#New-Features]

2. Supports all* Windows OS versions. Remote Access is handled by Virtualbox VRDP protocol.
3. Install and configure VboxVMService (see [http://vboxvmservice.sourceforge.net] to run Vboxwebsrv as a service. Currently Vboxwebsrv can only be configured natively to run as a service  on Linux and MAC.  

Screenshots





