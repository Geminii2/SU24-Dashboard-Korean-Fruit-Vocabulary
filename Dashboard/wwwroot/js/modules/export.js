
define(["jquery", "filesaver", "domtoimage"], function ($) {
	var Export = {}, Editor;

	$("#saveButton").click(function (event) {
		event.preventDefault(); // Prevent the default form submission
		var mapname = $('#Mapname').val();
		var description = $('#Description').val();

		$('.error-message').remove();

		if (!mapname) {
			// Display error message for Mapname
			$('#Mapname').after('<br/><span class="error-message" style="color: #ff5877">Mapname is required.</span>');
		}

		if (!description) {
			// Display error message for Description
			$('#Description').after('<br/><span class="error-message text-danger" style="color: #ff5877">Description is required.</span>');
		}

		// Check if any error messages were displayed
		if ($('.error-message').length > 0) {
			// Stop the form submission if there are validation errors
			return;
		}

		// Show loading screen
		$('#loading_screen').show();

		var exportedTxt = Export.file();
		var exportedImagePromise = Export.exportImage();
		exportedImagePromise.then(function (exportedImage) {
			// Create a FormData object and append the data
			var formData = new FormData();
			formData.append('mapName', mapname);
			formData.append('des', description);
			formData.append('txtFile', exportedTxt);
			formData.append('imageFile', exportedImage);
			// Append other form data as needed

			// Submit the form data via AJAX
			$.ajax({
				url: "/Map/Create",
				type: "POST",
				data: formData,
				contentType: false, // Important for sending files
				processData: false,
				success: function (response) {
					$('#loading_screen').hide();
					window.location.href = '/Profile/MyMap';
				},
				error: function (error) {
					console.log(error + " roi");
				}
			});
		}).catch(function (error) {
			console.log(error + "loi roi");
		});
	});

	$("#saveButtonAdmin").click(function (event) {
		event.preventDefault(); // Prevent the default form submission

		var mapname = $('#Mapname').val();
		var description = $('#Description').val();

		$('.error-message').remove();

		if (!mapname) {
			// Display error message for Mapname
			$('#Mapname').after('<br/><span class="error-message" style="color: #ff5877">Mapname is required.</span>');
		}

		if (!description) {
			// Display error message for Description
			$('#Description').after('<br/><span class="error-message" style="color: #ff5877">Description is required.</span>');
		}

		// Check if any error messages were displayed
		if ($('.error-message').length > 0) {
			// Stop the form submission if there are validation errors
			return;
		}

		$('#loading_screen').show();

		var exportedTxt = Export.adminfile();
		var exportedImagePromise = Export.exportImage();
		exportedImagePromise.then(function (exportedImage) {
			// Create a FormData object and append the data
			var formData = new FormData();
			formData.append('maptype', $('#Maptype').val());
			formData.append('mapName', mapname);
			formData.append('des', description);
			formData.append('txtFile', exportedTxt);
			formData.append('imageFile', exportedImage);
			// Append other form data as needed

			// Submit the form data via AJAX
			$.ajax({
				url: "/Admin/MapCreate",
				type: "POST",
				data: formData,
				contentType: false, // Important for sending files
				processData: false,
				success: function (response) {
					$('#loading_screen').hide();
					window.location.href = '/Admin/Map';	
				},
				error: function (error) {
					console.log(error + " roi");
				}
			});
		}).catch(function (error) {
			console.log(error + "loi roi");
		});
	});

	Export.initialize = function (namespace) {
		Editor = namespace;
		/*$("body").on("click", "#export", this.process);
		$("body").on("click", "#exportImage", this.exportImage);*/
		return this;
	};

	Export.file = function () {
		var type = "Matrix",
			tileset = Editor.active_tileset,
			w = $("#canvas").width() / tileset.tilesize.width,
			h = $("#canvas").height() / tileset.tilesize.height,
			output = "1\n25 30\n",
			y, x, query, coords, tilesetCoords;

		// Iterate through each layer
		$(".layer").each(function () {
			// Iterate through each tile in the layer
			for (y = 0; y < h; y++) {
				var row = "";
				for (x = 0; x < w; x++) {
					query = $(this).find("div[data-coords='" + x + "." + y + "']");
					coords = query.length ? query.attr("data-coords-tileset") : "Null";
					tilesetCoords = query.length ? query.attr("data-coords-tileset").split('.') : ["0", "0"];

					// If the coordinate is "0.0", replace it with "Null"
					coords = mapTilesetCoords(tilesetCoords);

					// Append the tileset coordinates to the output
					row += coords + " ";
				}
				row = row.trim();
				output += row + "\n"; // Newline after each row
			}
			output = output.trim() + "\n"; // Extra newline to separate layers

		});
		output = output.trim();
		// Save the output data as a text file
		var blob = new Blob([output], { type: "text/plain;charset=utf-8" });
		//saveAs(blob);
		return blob;
	};

	Export.adminfile = function () {
		var type = "Matrix",
			tileset = Editor.active_tileset,
			w = $("#canvas").width() / tileset.tilesize.width,
			h = $("#canvas").height() / tileset.tilesize.height,
			output = "1\n25 30\n",
			y, x, query, coords, tilesetCoords;

		// Iterate through each layer
		$(".layer").each(function () {
			// Iterate through each tile in the layer
			for (y = 0; y < h; y++) {
				var row = "";
				for (x = 0; x < w; x++) {
					query = $(this).find("div[data-coords='" + x + "." + y + "']");
					coords = query.length ? query.attr("data-coords-tileset") : "Null";
					tilesetCoords = query.length ? query.attr("data-coords-tileset").split('.') : ["0", "0"];

					// If the coordinate is "0.0", replace it with "Null"
					coords = mapTilesetCoordsAdmin(tilesetCoords);

					// Append the tileset coordinates to the output
					row += coords + " ";
				}
				row = row.trim();
				output += row + "\n"; // Newline after each row
			}
			output = output.trim() + "\n"; // Extra newline to separate layers

		});
		output = output.trim();
		// Save the output data as a text file
		var blob = new Blob([output], { type: "text/plain;charset=utf-8" });
		//saveAs(blob);
		return blob;
	};

	Export.exportImage = function () {
		var container = document.getElementById("canvas");
		var viewport = document.getElementById("viewport");
		var selection = document.getElementsByClassName("selection");

		var originalStyles = {
			container: container.style.cssText,
			viewport: viewport.style.cssText,
			selection: []
		};

		for (var i = 0; i < selection.length; i++) {
			originalStyles.selection[i] = selection[i].style.cssText;
		}

		// Hide elements before exporting
		viewport.style.visibility = "hidden";
		for (var i = 0; i < selection.length; i++) {
			selection[i].style.visibility = "hidden";
		}

		return new Promise(function (resolve, reject) {

			container.style.position = "static";
			for (var i = 0; i < selection.length; i++) {
				selection[i].style.position = "static";
			}

			domtoimage.toPng(container)
				.then(function (dataUrl) {
					var exportedImageData = dataURLtoBlob(dataUrl);
					//saveAs(exportedImageData);
					resolve(exportedImageData);
				})
				.catch(function (error) {
					reject(error);
				})
				.finally(function () {
					container.style.cssText = originalStyles.container;
					viewport.style.visibility = "visible";
					for (var i = 0; i < selection.length; i++) {
						selection[i].style.cssText = originalStyles.selection[i];
						selection[i].style.visibility = "visible";
					}
				});
		});
	}

	return Export;
});

function dataURLtoBlob(dataUrl) {
	var arr = dataUrl.split(','), mime = arr[0].match(/:(.*?);/)[1];
	var bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
	while (n--) {
		u8arr[n] = bstr.charCodeAt(n);
	}
	return new Blob([u8arr], { type: mime });
}

function mapTilesetCoords(tilesetCoords) {
	switch (tilesetCoords.join('.')) {
		case "0.0": return "Null";
		case "1.0": return "Wall";
		case "2.0": return "H_Bridge";
		case "3.0": return "V_Bridge";
		case "4.0": return "Ground";
		case "5.0": return "PlayerM:0";
		case "6.0": return "Ice";
		case "7.0": return "Socket_Green";
		case "0.1": return "Socket_Blue";
		case "1.1": return "Socket_Pink";
		case "2.1": return "Socket_Red";
		case "3.1": return "Socket_Orange";
		case "4.1": return "Socket_Indigo";
		case "5.1": return "Socket_Steal";
		case "6.1": return "Socket_Yellow";
		case "7.1": return "Socket_Purple";
		case "0.2": return "Socket_Azure";
		case "1.2": return "Socket_Aqua";
		default: return "Unknown";
	}
}

function mapTilesetCoordsAdmin(tilesetCoords) {
	switch (tilesetCoords.join('.')) {
		case "0.0": return "Null";
		case "1.0": return "Wall";
		case "2.0": return "H_Bridge";
		case "3.0": return "V_Bridge";
		case "4.0": return "Ground";
		case "5.0": return "PlayerM:0";
		case "6.0": return "PlayerF:1";
		case "7.0": return "Ice";
		case "0.1": return "Socket_Green";
		case "1.1": return "Socket_Blue";
		case "2.1": return "Socket_Pink";
		case "3.1": return "Socket_Red";
		case "4.1": return "Socket_Orange";
		case "5.1": return "Socket_Indigo";
		case "6.1": return "Socket_Steal";
		case "7.1": return "Socket_Yellow";
		case "0.2": return "Socket_Purple";
		case "1.2": return "Socket_Azure";
		case "2.2": return "Socket_Aqua";
		case "3.2": return "DoorButton:";
		case "4.2": return "Door";
		default: return "Unknown";
	}
}
