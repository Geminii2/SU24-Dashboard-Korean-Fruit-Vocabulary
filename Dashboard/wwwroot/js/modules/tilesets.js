define([
	"jquery-ui",
	"views/tilesets"
], function ($, TilesetView) {

	var Tilesets = {}, Editor, tilesetPath;
	Tilesets.collection = {};

	Tilesets.initialize = function (namespace) {

		Editor = namespace;
		this.view = TilesetView.initialize(Editor);
		var path = window.location.pathname;
		var isAdmin = path.includes("/Admin/");
		if (isAdmin) {
			tilesetPath = "/lib-map-editor/img/tilesets/AtuhConnectrix-admin.png";
		} else {
			tilesetPath = "/lib-map-editor/img/tilesets/AtuhConnectrix.png";
		}

		this.add(tilesetPath, {
			tilesize: { width: 32, height: 32 }
		});

		return this;
	};

	Tilesets.set = function (name) {

		var tileset = Tilesets.collection[name];
		Editor.active_tileset = tileset;

		$("#tileset_container").css({
			width: tileset.width,
			height: tileset.height,
		}).attr("class", "ts_" + tileset.id);

		$("#tilesets select").val(name);
		$("#tilesets .loading").remove();
		this.reset_selection();
	};

	Tilesets.add = function (src, opts) {

		var img = new Image(),
			bfr = document.createElement("canvas").getContext("2d"),
			name = opts.name || src.match(/(?:.+)\/([^\/]+)/)[1],
			style = style = document.createElement("style"),
			id = name.replace(/[^a-zA-Z]/g, '_'), css;

		img.src = src;
		img.addEventListener("load", function () {

			bfr.canvas.width = opts.width = this.width;
			bfr.canvas.height = opts.height = this.height;


			// Process tileset
			if (opts.alpha) { opts.base64 = Tilesets.setAlpha(this, opts.alpha); }
			if (opts.margin) { opts.base64 = Tilesets.slice(this, opts); }

			if (!opts.alpha && !opts.margin) {
				bfr.drawImage(this, 0, 0);
				opts.base64 = bfr.canvas.toDataURL();
			}

			opts.id = id;
			opts.name = name;

			Tilesets.collection[name] = opts;
			Tilesets.set(name);

			// Add a global css class so tiles can use
			// it in conjunction with background-position
			$(style).attr("id", "tileset_" + id);
			css = ".ts_" + id + ", .ts_" + id + " > div {\n";
			css += "\twidth: " + opts.tilesize.width + "px;\n";
			css += "\theight: " + opts.tilesize.height + "px;\n";
			css += "\tbackground-image: url('" + opts.base64 + "');\n";
			css += "}";
			$(style).append(css);

			$("head").append(style);

			// Update select element
			$("#tilesets select").append("<option>" + name + "</option>");
			$("#tilesets select").val(name);

			// Update custom scrollbars and grid
			$("#tileset").jScrollPane();
			Editor.Canvas.update_grid();

		}, false);

	};

	// Filters specified color and makes it transparent
	Tilesets.setAlpha = function (img, alpha) {
		var bfr = document.createElement("canvas").getContext("2d"),
			imgData, red, i, l;

		bfr.canvas.width = img.width;
		bfr.canvas.height = img.height;
		bfr.drawImage(img, 0, 0);

		imgData = bfr.getImageData(0, 0, img.width, img.height);
		tolerance = 10;

		for (i = 0, l = imgData.data.length; i < l; i++) {
			red = i % 4 == 0 ? true : false;

			if (red) {
				if (
					imgData.data[i] >= alpha[0] - tolerance && imgData.data[i] <= alpha[0] + tolerance &&
					imgData.data[i + 1] >= alpha[1] - tolerance && imgData.data[i + 1] <= alpha[1] + tolerance &&
					imgData.data[i + 2] >= alpha[2] - tolerance && imgData.data[i + 2] <= alpha[2] + tolerance

				) {
					imgData.data[i + 3] = 0;
				}
			}
		}

		bfr.clearRect(0, 0, img.width, img.height);
		bfr.putImageData(imgData, 0, 0);
		return bfr.canvas.toDataURL();
	};

	// Slices the tileset according to tile size and margin
	Tilesets.slice = function (img, opts) {

		var bfr = document.createElement("canvas").getContext("2d"),
			tw = opts.tilesize.width,
			th = opts.tilesize.height,
			imgData, red,
			x, y, xl, yl,
			m = opts.margin;

		bfr.canvas.width = img.width - (img.width / tw) * opts.margin;
		bfr.canvas.height = img.height - (img.height / th) * opts.margin;

		for (y = 0, ly = Math.floor(bfr.canvas.height / th); y < ly; y++) {
			for (x = 0, lx = Math.floor(bfr.canvas.width / tw); x < lx; x++) {
				bfr.drawImage(
					img,
					(x * (tw + m)) + m,
					(y * (th + m)) + m,
					tw, th,
					x * tw,
					y * th,
					tw, th
				);
			}
		}

		return bfr.canvas.toDataURL();
	};

	Tilesets.reset_selection = function () {
		$("#canvas .selection").remove();
		$("#tileset .selection").remove();
		delete Editor.selection;
	};

	Tilesets.get_active = function () { return Tilesets.collection[$("#tilesets select option:selected").val()]; }

	Tilesets.get_infobox = function (event) {
		console.log("Mouseover event triggered");

		// Get the mouse coordinates relative to the canvas
		var rect = event.target.getBoundingClientRect();
		var x = event.clientX - rect.left;
		var y = event.clientY - rect.top;

		var blockName = mapTilesetCoords([x, y]);
		console.log("Block Name:", blockName);

		// Display a tooltip or information box with the block information
		displayBlockInfoTooltip(event, blockName);
	};


	return Tilesets;
});
