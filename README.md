RapidFTP
========

This is wrapper for Chilkat (x86 only).

You can view some samples in FtpClientTest.

Currently i only support this following freature
- FTP protocol (Plain FTP, FTPS, FTPES)
- SFTP protolcol
- Download, Upload, List, Delete, Exist command
- Integration test with Xlight server. (I also check with FileZilla server and it work without problem) 
    + Port 40000: PlainFTP
    + Port 40001: FTPS
    + Port 40002: FTPES
    + Port 40003: SFTP

The ChilkatLicense.ini hold your license key for Chilkat. You must create it under users/ChilkatLicense.ini to able to run unit test. See UnlockComponentFixture for the setting template.