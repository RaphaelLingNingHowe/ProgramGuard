﻿export default class Fetcher {
    async fetchJson(resource, options) {
        var ret;
        if (!options)
            options = {};
        if (!options.credentials)
            options.credentials = "same-origin";
        if (!options.method)
            options.method = "POST";
        if (!options.headers)
            options.headers = {};
        if (!options.headers["Content-Type"])
            if (!options.body || typeof options.body === "string")
                options.headers["Content-Type"] = "application/json";

        try {
            const response = await fetch(resource, options);
            if (!response.ok) {
                var message = await response.text();
                if (!message)
                    message = response.statusText;
                var tmp;
                try {
                    tmp = JSON.parse(message); // check if response is a json object
                } catch (e) {
                    tmp = { Result: false, Message: message };
                }

                if (!tmp.StatusCode)
                    tmp.StatusCode = response.status;

                if (!tmp.Message && !tmp.Result && tmp.StatusCode != 401)
                    tmp.Message = "Generic fetch error";

                if (tmp.Result === undefined) { // if undefined, the error comes from middleware
                    var m;
                    if (tmp.title)  // title is a property generated by .net core middleware, contains the actual error.
                        m = tmp.title;
                    else
                        m = message;
                    if (tmp.errors) {
                        var sp = "";
                        for (var p in tmp.errors)
                            sp += tmp.errors[p] + "\n";
                        if (sp)
                            m += "\n" + sp;
                    }
                    if (m && m.length > 300)
                        m = m.substr(0, 300);
                    ret = { Result: false, Message: m };
                }
                else // otherwise, it could be a backend generated (controlled) response
                    ret = tmp;

                // this checks if the response is a dump of a page sent by the host (i.e. IIS)
                if (ret.Message && ret.Message.toLowerCase().indexOf("<!doctype") >= 0)
                    ret.IsResponsePageResult = true;
                else
                    ret.IsResponsePageResult = false;
            }
            else {
                // if response is ok, then its up to the backend to return an object that has a .Result property or not
                // depending on the caller. For example, file contents should be plain data, without an added .Result property
                const contentType = response.headers.get("content-type");
                if (contentType && contentType.indexOf("application/json") !== -1) {
                    ret = { StatusCode: 200, Result: true, Data: await response.json() };
                }
                else {
                    ret = { StatusCode: 200, Result: false, Message: await response.text() };
                    if (ret.Message && ret.Message.toLowerCase().indexOf("<!doctype") >= 0)
                        ret.IsResponsePageResult = true;
                    else
                        ret.IsResponsePageResult = false;
                }
            }
            return ret;
        }
        catch (e) {
            DevExpress.ui.notify("網路異常, " + e, "error", 3000);
        }
    }

    async fetchData(resource, options) {
        var ret;
        if (!options)
            options = {};
        if (!options.credentials)
            options.credentials = "same-origin";
        if (!options.method)
            options.method = "POST";
        if (!options.headers)
            options.headers = {};
        if (!options.headers["Content-Type"])
            if (!options.body || typeof options.body === "string")
                options.headers["Content-Type"] = "application/json";

        try {
            const response = await fetch(resource, options);
            if (!response.ok) {
                var message = await response.text();
                if (!message)
                    message = response.statusText;
                var tmp;
                try {
                    tmp = JSON.parse(message); // check if response is a json object
                } catch (e) {
                    tmp = { Result: false, Message: message };
                }

                if (!tmp.StatusCode)
                    tmp.StatusCode = response.status;

                if (!tmp.Message && !tmp.Result && tmp.StatusCode != 401)
                    tmp.Message = "Generic fetch error";

                if (tmp.Result === undefined) { // if undefined, the error comes from middleware
                    var m;
                    if (tmp.title)  // title is a property generated by .net core middleware, contains the actual error.
                        m = tmp.title;
                    else
                        m = message;
                    if (tmp.errors) {
                        var sp = "";
                        for (var p in tmp.errors)
                            sp += tmp.errors[p] + "\n";
                        if (sp)
                            m += "\n" + sp;
                    }
                    if (m && m.length > 300)
                        m = m.substr(0, 300);
                    ret = { Result: false, Message: m };
                }
                else // otherwise, it could be a backend generated (controlled) response
                    ret = tmp;

                // this checks if the response is a dump of a page sent by the host (i.e. IIS)
                if (ret.Message && ret.Message.toLowerCase().indexOf("<!doctype") >= 0)
                    ret.IsResponsePageResult = true;
                else
                    ret.IsResponsePageResult = false;
            }
            else {
                // if response is ok, then its up to the backend to return an object that has a .Result property or not
                // depending on the caller. For example, file contents should be plain data, without an added .Result property
                const contentType = response.headers.get("content-type");
                if (contentType && contentType.indexOf("application/zip") !== -1 || contentType.indexOf("audio/wav") !== -1) {
                    let fileName = "";

                    const disposition = response.headers.get("content-disposition")
                    if (disposition && disposition.indexOf('attachment') !== -1) {
                        var fileNameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                        var matches = fileNameRegex.exec(disposition);
                        if (matches != null && matches[1]) {
                            fileName = matches[1].replace(/['"]/g, '');
                        }
                    }

                    ret = { StatusCode: 200, Result: true, FileName: fileName, Blob: await response.blob() };
                }
                else if (contentType && contentType.indexOf("application/json") !== -1) {
                    ret = { StatusCode: 200, Result: true, Data: await response.json() };
                }
                else {
                    ret = { StatusCode: 200, Result: false, Message: await response.text() };
                    if (ret.Message && ret.Message.toLowerCase().indexOf("<!doctype") >= 0)
                        ret.IsResponsePageResult = true;
                    else
                        ret.IsResponsePageResult = false;
                }
            }
            return ret;
        }
        catch (e) {
            DevExpress.ui.notify("網路異常, " + e, "error", 3000);
        }
    }
}
