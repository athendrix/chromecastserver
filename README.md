This is a Chromecast server.

Links should take the form http://<This Server>/cast?receiver=<Chromecast Device IP>&mi=<Path To Media>&random=true&repeat=true
	
You can add many mi entries.

It is recommended to give your Chromecast devices static IP reservations.

If random is set to true, the media files passed in will be loaded in a random order, and the Chromecast should shuffle on repeat

If repeat is set to true, the media will repeat the playlist on completion. To repeat one, only load one file.

Future versions will support:
 - Web folder paths in addition to just files
 - POST requests in addition to GET requests
	
Hostnames do not work for Chromecast devices because DNS for containers is often different.

Hence why static IP reservations is recommended.
