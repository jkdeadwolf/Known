﻿var DevDatabase = {

    grid: null,
    toolbar: null,

    show: function () {
        this.grid = new Grid('Database');
        this.toolbar = new Toolbar('tbDatabase', this);
    },

    //toolbar
    new: function () {
        this.grid.query.clear();
        this.grid.setColumns([]);
    },

    query: function () {
        if (!this.grid.query.validate())
            return;

        var grid = this.grid;
        grid.load(function (e) {
            var columns = [{ type: "indexcolumn" }];
            if (e.result.data.length) {
                for (var p in e.result.data[0]) {
                    if (p === '_id' || p === '_uid') {
                        continue;
                    }
                    columns.push({ field: p, header: p });
                }
            }
            grid.setColumns(columns);
            new ColumnsMenu(e.sender);
        });
    }

};