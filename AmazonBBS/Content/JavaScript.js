"v0.4.6 Geetest Inc.";
function check_enter(n) {
    if (n.keyCode == 13) {
        var t = n.target || n.srcElement;
        if (t.id == "input1") {
            if (document.getElementById("input1").value == "") {
                $("#tip_input1").html("请输入登录用户名");
                return
            }
            if (document.getElementById("input2").value == "") {
                document.getElementById("input2").focus();
                return
            }
        }
        if (t.id == "input2" && document.getElementById("input2").value == "") {
            $("#tip_input2").html("请输入密码");
            return
        }
        signin()
    }
}
function signin() {
    var t = $.trim($("#input1").val()),
    n;
    if (!t) {
        $("#tip_input1").html("请输入登录用户名");
        $("#input1").focus();
        return
    }
    if (n = $.trim($("#input2").val()), !n) {
        $("#tip_input2").html("请输入密码");
        $("#input2").focus();
        return
    }
    common.loadingBtn = Ladda.create(document.getElementById("signin"));
    common.loadingBtn.start();
    geetest.dealError();
    geetest.isRefresh && geetest.getcaptchaObj.refresh();
    geetest.isRefresh = !1;
    $("#checkWay").modal("show");
    $("#checkWay").on("hidden.bs.modal",
    function () {
        common.loadingBtn.stop()
    })
}
function signinGo() {
    var r, u, f, n, i;
    if (!is_in_progress) {
        if ($("#tip_input1").html(""), $("#tip_input2").html(""), r = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDKJPFz0k33Xq4fCKDNQpn/ttUhLLmajOKBhVe0idsvk3rrNN6N5ESosOpd+jZ+8DQrwGQKGbDd8is5qBi5egRa6fJvTxIxj55ZkuhUmcSHMJd9CpDQhZ/9Vmh8N3/lHailfoWZTwD9SDsVqlLrqnmHBKqbzJ7q5mR09LgciUkgkwIDAQAB", u = $.trim($("#input1").val()), !u) {
            $("#tip_input1").html("请输入登录用户名");
            $("#input1").focus();
            return
        }
        if (f = $.trim($("#input2").val()), !f) {
            $("#tip_input2").html("请输入密码");
            $("#input2").focus();
            return
        }
        $("#tip_btn").html("提交中...");
        n = new JSEncrypt;
        n.setPublicKey(r);
        var e = n.encrypt($("#input1").val()),
        o = n.encrypt($("#input2").val()),
        t = {
            input1: e,
            input2: o,
            remember: $("#remember_me").prop("checked"),
            geetest_challenge: $("[name='geetest_challenge']").val(),
            geetest_validate: $("[name='geetest_validate']").val(),
            geetest_seccode: $("[name='geetest_seccode']").val()
        };
        enable_captcha && (i = $("#captcha_code_input").get(0).Captcha, t.captchaId = i.Id, t.captchaInstanceId = i.InstanceId, t.captchaUserInput = $("#captcha_code_input").val());
        is_in_progress = !0;
        $.ajax({
            url: ajax_url,
            type: "post",
            data: JSON.stringify(t),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: {
                VerificationToken: "@TokenHeaderValue()"
            },
            success: function (n) {
                n.success ? ($("#tip_btn").html("登录成功，正在重定向..."), location.href = return_url) : (dealError(), $("#tip_btn").html(n.message + "<br/><br/>联系 contact@cnblogs.com"), is_in_progress = !1)
            },
            error: function () {
                is_in_progress = !1;
                dealError();
                $("#tip_btn").html("抱歉！出错！联系 contact@cnblogs.com")
            }
        })
    }
}
function setFocus() {
    document.getElementById("input1").focus()
}
function dealError() {
    geetest.reset();
    $("#checkWay").modal("hide");
    $("#showLoading").hide()
} (function (n) {
    "use strict";
    function r(n) {
        this._obj = n
    }
    function f(n) {
        var t = this;
        new r(n)._each(function (n, i) {
            t[n] = i
        })
    }
    if (typeof n == "undefined") throw new Error("Geetest requires browser environment");
    var u = n.document,
    c = n.Math,
    l = u.getElementsByTagName("head")[0];
    r.prototype = {
        _each: function (n) {
            var i = this._obj,
            t;
            for (t in i) i.hasOwnProperty(t) && n(t, i[t]);
            return this
        }
    };
    f.prototype = {
        api_server: "api.geetest.com",
        protocol: "http://",
        typePath: "/gettype.php",
        fallback_config: {
            slide: {
                static_servers: ["static.geetest.com", "dn-staticdown.qbox.me"],
                type: "slide",
                slide: "/static/js/geetest.0.0.0.js"
            },
            fullpage: {
                static_servers: ["static.geetest.com", "dn-staticdown.qbox.me"],
                type: "fullpage",
                fullpage: "/static/js/fullpage.0.0.0.js"
            }
        },
        _get_fallback_config: function () {
            var n = this;
            return e(n.type) ? n.fallback_config[n.type] : n.new_captcha ? n.fallback_config.fullpage : n.fallback_config.slide
        },
        _extend: function (n) {
            var t = this;
            new r(n)._each(function (n, i) {
                t[n] = i
            })
        }
    };
    var a = function (n) {
        return typeof n == "number"
    },
    e = function (n) {
        return typeof n == "string"
    },
    v = function (n) {
        return typeof n == "boolean"
    },
    o = function (n) {
        return typeof n == "object" && n !== null
    },
    y = function (n) {
        return typeof n == "function"
    },
    t = {},
    i = {},
    p = function () {
        return parseInt(c.random() * 1e4) + (new Date).valueOf()
    },
    w = function (n, t) {
        var i = u.createElement("script"),
        r;
        i.charset = "UTF-8";
        i.async = !0;
        i.onerror = function () {
            t(!0)
        };
        r = !1;
        i.onload = i.onreadystatechange = function () {
            r || i.readyState && "loaded" !== i.readyState && "complete" !== i.readyState || (r = !0, setTimeout(function () {
                t(!1)
            },
            0))
        };
        i.src = n;
        l.appendChild(i)
    },
    b = function (n) {
        return n.replace(/^https?:\/\/|\/$/g, "")
    },
    k = function (n) {
        return n = n.replace(/\/+/g, "/"),
        n.indexOf("/") !== 0 && (n = "/" + n),
        n
    },
    d = function (n) {
        if (!n) return "";
        var t = "?";
        return new r(n)._each(function (n, i) {
            (e(i) || a(i) || v(i)) && (t = t + encodeURIComponent(n) + "=" + encodeURIComponent(i) + "&")
        }),
        t === "?" && (t = ""),
        t.replace(/&$/, "")
    },
    g = function (n, t, i, r) {
        t = b(t);
        var u = k(i) + d(r);
        return t && (u = n + t + u),
        u
    },
    s = function (n, t, i, r, u) {
        var f = function (e) {
            var o = g(n, t[e], i, r);
            w(o,
            function (n) {
                n ? e >= t.length - 1 ? u(!0) : f(e + 1) : u(!1)
            })
        };
        f(0)
    },
    nt = function (t, i, r, u) {
        if (o(r.getLib)) {
            r._extend(r.getLib);
            u(r);
            return
        }
        if (r.offline) {
            u(r._get_fallback_config());
            return
        }
        var f = "geetest_" + p();
        n[f] = function (t) {
            t.status == "success" ? u(t.data) : t.status ? u(r._get_fallback_config()) : u(t);
            n[f] = undefined;
            try {
                delete n[f]
            } catch (i) { }
        };
        s(r.protocol, t, i, {
            gt: r.gt,
            callback: f
        },
        function (n) {
            n && u(r._get_fallback_config())
        })
    },
    h = function (n, t) {
        var i = {
            networkError: "网络错误",
            gtTypeError: "gt字段不是字符串类型"
        };
        if (typeof t.onError == "function") t.onError(i[n]);
        else throw new Error(i[n]);
    },
    tt = function () {
        return n.Geetest || u.getElementById("gt_lib")
    };
    tt() && (i.slide = "loaded");
    n.initGeetest = function (r, u) {
        var e = new f(r);
        r.https ? e.protocol = "https://" : r.protocol || (e.protocol = n.location.protocol + "//"); (r.gt === "050cffef4ae57b5d5e529fea9540b0d1" || r.gt === "3bd38408ae4af923ed36e13819b14d42") && (e.apiserver = "yumchina.geetest.com/", e.api_server = "yumchina.geetest.com");
        o(r.getType) && e._extend(r.getType);
        nt([e.api_server || e.apiserver], e.typePath, e,
        function (r) {
            var f = r.type,
            c = function () {
                e._extend(r);
                u(new n.Geetest(e))
            },
            o;
            t[f] = t[f] || [];
            o = i[f] || "init";
            o === "init" ? (i[f] = "loading", t[f].push(c), s(e.protocol, r.static_servers || r.domains, r[f] || r.path, null,
            function (n) {
                var u, r, s, o;
                if (n) i[f] = "fail",
                h("networkError", e);
                else {
                    for (i[f] = "loaded", u = t[f], r = 0, s = u.length; r < s; r = r + 1) o = u[r],
                    y(o) && o();
                    t[f] = []
                }
            })) : o === "loaded" ? c() : o === "fail" ? h("networkError", e) : o === "loading" && t[f].push(c)
        })
    }
})(window);
var geetest = function (n) {
    function t(t) {
        geetest.isError = !1;
        geetest.isRefresh = !1;
        geetest.geetestCallback = t;
        n.ajax({
            url: "/geetst/GeetestParam?" + Math.random(),
            type: "get",
            dataType: "json",
            success: function (n) {
                initGeetest({
                    gt: n.gt,
                    challenge: n.challenge,
                    product: "embed",
                    offline: !n.success
                },
                u)
            },
            error: function () {
                alert("验证码参数获取错误")
            }
        })
    }
    function r() {
        geetest.isError && (n("#captchaBox").html("<span> 验证码组件加载中,请稍后...<\/span>"), t(geetest.geetestCallback))
    }
    function u(t) {
        n("#captchaBox").empty();
        t.appendTo("#captchaBox");
        t.onSuccess(geetest.geetestCallback);
        t.onError(function () {
            geetest.isError = !0
        });
        geetest.getcaptchaObj = t
    }
    function f() {
        this.getcaptchaObj.reset()
    }
    var i, e = function () { };
    return {
        dealError: r,
        getcaptchaObj: i,
        isError: !1,
        isRefresh: !1,
        getParam: t,
        reset: f
    }
}($),
common = {
    loadingBtn: null,
    showLoading: null,
    submitError: function (n) {
        n.status == 400 ? this.getError(n.status + " error-页面凭证或已过期被拒绝,请刷新页面重试!") : n.status == 404 ? this.getError(n.status + " error-所访问的资源不存在!") : this.getError(n.status + " error-请联系:contact@cnblogs.com")
    },
    getError: function (n) {
        this.loadingBtn && this.loadingBtn.stop();
        $("#tip_btn").html(n);
        showLoading && (geetest.isRefresh = !0, $("#checkWay").modal("hide"), this.showLoading.stop(), $("#showLoading").hide())
    }
};
$(function () {
    geetest.getParam(signinGo);
    $("#signin").bind("click",
    function () {
        signin()
    }).val("登 录")
})