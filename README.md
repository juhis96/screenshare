# screenshare
A small project for fun. C#, PHP and a bit of JS.

Out of curiosity for how malware is capable of spying on its victim's monitor, I decided to make a sample project in C# and PHP.

The program first takes a screenshot as a Bitmap, but rather than saving it, the screenshot is first resized and the quality is lowered by 50% to achieve a faster data transfer (which is important for the refresh rate). Then the Bitmap is encrypted with Base64 into a string. The Base64 string is then sent over to an external Web Server with a POST method. The PHP file receiving the string simply decrypts it and saves the Bitmap as a JPEG image, which is then displayed on the web server. As the program keeps sending new screenshots, the previous ones get overwritten to prevent filling up the server with unnecessary images. The JavaScript included in the PHP script refreshes the screenshot for the viewer. The end result is a low framerate stream of screenshots, but good enough for seeing what's being done on the computer.

Demo: https://www.youtube.com/watch?v=Y7Zd4i9eGa4
