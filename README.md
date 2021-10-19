# CoronaTracker <img src="http://coronatracker.ncodes.eu/coronavirus.png" width="50" height="50">
Seminar school project.

## Introduce CoronaTracker
This program was created for seminar school project as recession to Corona virus. Program is develop for non-exists hospital. It saved patients and their data about covid data. In databa I store Employees data such as fullname, email, phone, pose, etc. Then Vaccinate, Finds, Vaccine types. And lastable program data such as version and download link to lastest release.

**Data in database are fully generated!**

## Some features that are worth to stab:
- NuGet
	- MySQL connector for easier connection
	- HTML to PDF converter
	- And soome GUI nuget stuff
- RestAPI
	- Using RestAPI to get status about corona
	- There was a bit problem while using it. Old one was removed, so I have to use new one
- MySQL Database
	- Every data is saved in MySQL database
	- Database methods are asynchrous
- Login System
	- In sign in / sign up using SHA256 with 2 salts
	- Can update password in settings
	- Auto login sessions included
	- Reset password via email code
- Polyformism
- Github Connection
- Structure
- App Installer
- Others
	- Automatically send emails via own domain
	- Generate PDF file from HTML model
	- Automatically refresh database connection (to be safe)
	- Dashboard data science
	- QR code webcam loader
	- Check version correction

## To-Do list (15/15)
- [:heavy_check_mark:] Create main UI form
- [:heavy_check_mark:] Create login UI form
- [:heavy_check_mark:] Create Home sub form
- [:heavy_check_mark:] Create Dashboard sub form
- [:heavy_check_mark:] Create Countries sub form
- [:heavy_check_mark:] Create Patient sub form
- [:heavy_check_mark:] Create Vaccine sub form
- [:heavy_check_mark:] Create Settings sub form
- [:heavy_check_mark:] Create MySQL connection
- [:heavy_check_mark:] Connect sub forms with MySQL
- [:heavy_check_mark:] Create RestAPI methods
- [:heavy_check_mark:] Add QR load patient
- [:heavy_check_mark:] Comment whole source code
- [:heavy_check_mark:] Add print files (finds doc / vaccinate doc)
- [:heavy_check_mark:] First release with installer

## Some little idea to do
- Reset password through email [:heavy_check_mark:]
- Send print files through email
- Login after push enter

## Used technologies
- [Old RestAPI - TrackCorona](https://www.trackcorona.live)
- [New RestAPI - Gramzivi Covid API](https://rapidapi.com/Gramzivi/api/covid-19-data/)
- [Used icons - Flaticon](https://www.flaticon.com/free-icon/)
- [To see some documentaries - Github](https://github.com)
- [To copy some codes - StackOverflow](https://stackoverflow.com)
- [To learn about web - W3Schools](https://www.w3schools.com)
- [Generate data - Mockaroo](https://www.mockaroo.com)
- [To create qr codes - QR code API](https://goqr.me/api/)
