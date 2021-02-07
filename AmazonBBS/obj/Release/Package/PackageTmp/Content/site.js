"use strict"
var Leo = Leo || {
    _pk: +new Date,
    baseUrl: window.location.origin || "http://www.baixiaotangtop.com",
    gologin: function (returnUrl) { location.href = this.baseUrl + "/account/login?returnurl={0}".Format(returnUrl ? returnUrl.indexOf(this.baseUrl) > -1 ? returnUrl : this.baseUrl + returnUrl : location.href); },
    getPK: function () { return ++this._pk },
    init: function (option, action, p) {
        var t = this;
        return $ && $(document).ready(function () {
            if (!t.init[action]) {
                t.init[action] = !0;
                action ? ((t[option] && t[option]()(action, p))) : ((t[option] && (t[option](), delete t[option])));
                if (option.toString().toLowerCase() != "user") {
                    if (action && action.toString().toLowerCase() === "detail") {
                        var _mid = t.tools.isArray(p) ? p[0] : t.tools.isObject(p) ? p["id"] : p;
                        var paths = location.pathname.split("/");
                        $.post("/pv/record/{0}".Format(_mid === paths[3] ? _mid : paths[3]), { pvenum: paths[1] });
                    }
                }
                $(".btn_login").unbind("click").click(function (e) {
                    e.preventDefault();
                    //location.href = "/account/login?returnurl={0}".Format(location.href);
                    var url = $(this).attr("url");
                    t.gologin(url);
                });
                t.BT.Popover.call(t);
                t.ToolTip();
                //接收消息
                //主页，个人中心
                if ((option == "home") || (option == "user")) {
                    t.ReceiveMessage();
                }
                t.HightLightSelectArea();
                t.Hide_Wechat_QQ_GZH();
                //t.InitImage();
            }
        }), t;
    },
    //隐藏左侧微信公众号，微信、qq客服 
    Hide_Wechat_QQ_GZH: function () {
        var t = this;
        var browser = t.browser;
        if (browser.versions.ios || browser.versions.android || browser.versions.iPhone || browser.versions.iPad) {
            $(".kefu_icon").hide();
        }
    },
    InitImage: function () {
        $(".boxtable td.plc img").css("max-width", $(".boxtable td.plc").width() - 20);
    },
    TableRemoveStyle: function () {
        $("table").removeAttr("style");
    },
    ToolTip: function () {
        $("[data-toggle='tooltip']").tooltip();
    },
    //角落弹穿
    MsgTips: function (title, msg, offset) {
        layer.open({
            type: 1
            , area: ["240px", "auto"]
            , title: title
            , offset: offset || 'rb' //具体配置参考：offset参数项
            , content: "<div style='padding:20px'>{0}</div>".Format(msg)
            , btn: '我知道了'
            , btnAlign: 'c' //按钮居中
            , shade: 0 //不显示遮罩
            , yes: function (index) {
                layer.close(index)
            }
        });
    },
    InitBodyTop: function () {
        function run() {
            var navHeight = $("body").find(".navbar").height();
            $("body").css("padding-top", navHeight);
        }
        run();
        $(window).resize(function () {
            run();
        });
    },
    InitGoTopButton: function () {
        var toping = false;
        $(document).scroll(function () {
            if ($(document).scrollTop() > 300) {
                if ($(".arrow-box").length == 0) {
                    $("body").append($("<div>").addClass("arrow top arrow-box").attr("title", "回到顶部").click(function () {
                        toping = true;
                        $("body,html").animate({ scrollTop: 0 }, 300);
                        $(".arrow-box").hide(function () { toping = false; });
                    }).append($("<b>").addClass("top").append($("<i>").addClass("arrow1 top"))));
                } else {
                    if (!toping) {
                        $(".arrow-box").show();
                    }
                }
            } else {
                $(".arrow-box").hide(function () { toping = false; });
            }
        });
    },
    ShowMaxImg: function (element, shift) {
        var obj = { photos: element || '.layer-photos-wrap' };
        !isNaN(shift) && (obj.shift = shift)//0-6的选择，指定弹出图片动画类型，默认随机
        layer.ready(function () { //为了layer.ext.js加载完毕再执行
            layer.photos(obj);
        });
    },
    /* 移除 img 外面的 a 标签的href */
    RemoveImageWrapperHref: function () {
        var imgs = $(".boxtable .plc .pct").find("img");
        $.each(imgs, function (i, img) {
            var parent = $(this).parent().get(0);
            if (parent.nodeName == "A" && parent.href.indexOf("baixiaotangtop.com") == -1) {
                $(parent).removeAttr("href");
            }
        });
    },
    /*
        bootstrap的标签页切换加载数据及分页
        @params tabsWrapElement :需要滚动顶端的div
        @params rendDataElement :载入数据的div
        @params btnPageElement :分页的按钮div
        @params url :请求数据的url
        @params loadPosition_Margin :加载遮罩层的margin位置
        @params loadPosition_Top :加载遮罩层的top位置
    */
    BootStrap_Tab_Change: function (tabsWrapElement, rendDataElement, btnPageElement, url, loadUrlParams, loadPosition_Margin, loadPosition_Top, callback, tabChange) {
        loadPosition_Margin = loadPosition_Margin || "-15px 0";
        loadPosition_Top = loadPosition_Top || "15px";
        var t = this;
        function Mask() {
            return t.LoadMask(rendDataElement, loadPosition_Margin, loadPosition_Top);
        }
        function InitPageAction(sort) {
            t.Page.PageRowNumber(btnPageElement, rendDataElement, "{0}{2}t={1}".Format(url, t.getPK(), url.indexOf("?") > -1 ? "&" : "?"), tabsWrapElement, function () {
                return Mask();
            }, sort, callback);
        }

        $('{0} a[data-toggle="tab"]'.Format(tabsWrapElement)).on("shown.bs.tab", function (e) {
            var layerIndex = Mask();
            var activeTab = $(e.target).data("reactid");
            t.ScrollTop(tabsWrapElement);
            //setTimeout(function () {
            $.get("{0}&t={1}".Format(url, t.getPK()).Format(activeTab), function (data) {
                $(rendDataElement).empty().append(data);
                t.ToolTip();
                btnPageElement && InitPageAction(activeTab);
                t.LoadMask.Remove(layerIndex);
            }).error(function () { t.LoadMask.Remove(layerIndex); });
            tabChange && tabChange()
            //}, t.tools.Random(500, 1000));
        });
        btnPageElement && InitPageAction(loadUrlParams || "");
    },
    //高亮选中区域
    HightLightSelectArea: function () {
        var hash = location.hash;
        //$(hash).addClass("background-color", "red");
        var target = $(hash);
        if (target.length > 0) {
            var time = 2000;
            var start = 150;
            var step = 5.25;
            var timeInterval = setInterval(function () {
                if (time > 0) {
                    start = start + step;
                    target.css("background-color", "rgb(255,255,{0})".Format(start));
                    time = time - 100;
                } else {
                    clearInterval(timeInterval);
                    target.css("background-color", "initial");
                }
            }, 100);
        }
    },
    //搜索
    Search: function (callback) {
        var t = this;
        function search() {
            var searchValue = ".searchValue";
            if (t.tools.CheckFormNotEmpty(searchValue, "搜索内容", !0)) {
                callback(searchValue.val());
            }
        }
        $(".btn-search-leo").unbind("click").click(function () {
            search();
        });
        t.onfocusKeyup($(".searchValue"), t.keyup.onKeyUpType.Enter, function () {
            search();
        });
    },
    InitLikeControl: function (a) {
        var t = this, type = arguments[0], mid = arguments[1];
        function Un() {
            $(".btn_unlike").length > 0 && (
                $(".btn_unlike").unbind("click").click(function () {
                    Like.call(this, 1);
                })
                    .mouseover(function () {
                        $(this).text("取消关注")
                    })
                    .mouseleave(function () {
                        $(this).text("已关注")
                    }),
                $(".btn_like").unbind("click"));
        }
        function L() {
            $(".btn_like").length > 0 && ($(".btn_like").unbind("click").click(function () {
                Like.call(this);
            })
                .mouseover(function () {
                    $(this).text("关注")
                })
                .mouseleave(function () {
                    $(this).text("关注")
                }),
                $(".btn_unlike").unbind("click"));
        }
        function Like() {
            var $this = $(this);
            var f = arguments.length > 0;
            $.post("/bbs/like?t=" + t.getPK(), {
                id: $this.parent().data("mid") == mid ? mid : $this.parent().data("mid"), type: type, unlike: f
            }, function (rd) {
                if (rd.Ok) {
                    if (f) {
                        $this.addClass("btn_like").removeClass("btn_unlike").text("关注");
                        L();
                    } else {
                        $this.addClass("btn_unlike").removeClass("btn_like").text("取消关注");
                        Un();
                    }
                    $this.blur();
                    $(".likecount").length && $(".likecount").text(f ? (parseInt($(".likecount").text()) - 1) : (parseInt($(".likecount").text()) + 1))
                    layer.msg(rd.Msg);

                } else {
                    layer.msg(rd.Msg || "失败");
                }
            }).error(function (data) {
                alert(data.Msg || "提交异常，稍后再试!");
            });
        }
        Un(); L();
    },
    LoadCommentControl: function () {
        var t = this, time = 650, cflag = !1;
        return $(".btn_comment").unbind("click").click(function () {
            if (t.user.login) {
                $(".note-editor").length ? (
                    cflag ? (cflag = !1, $(".note-editor,.commentAreaItems").hide(time), this.innerText = "评论",
                        $("#needScore").attr("checked", !1), $("#payScore").attr("disabled", !0).val("")
                    ) :
                        (cflag = !0, $(".note-editor,.commentAreaItems").show(time, function () { $("body,html").animate({ scrollTop: $('#summernote').parent().offset().top - 50 }, 200), $('#summernote').summernote("focus") }), this.innerText = "暂不评论"))
                    : (
                        cflag = !0,
                        this.innerText = "暂不评论"
                        , (t.RichText("#summernote", "/tool/uploadimg", "请发挥你的见解", 300, 0, function (data) {
                            $("#summernote").summernote('insertImage', data.Url);
                        }, function () { $(".note-editor").hide() }, !0)),
                        $(".note-editor").show(time, function () {
                            $(".commentAreaItems").removeClass("Ldn");
                        }),
                        $("#needScore").unbind("change").change(function () {
                            $("#payScore").attr("disabled", this.checked ? !1 : !0).val("");
                        }),
                        $(".btn_Answer").unbind("click").click(function () {
                            var _btn_me = $(this);
                            if ($('#summernote').summernote("isEmpty")) {
                                layer.msg("评论内容不能为空");
                                $("body,html").animate({ scrollTop: $('#summernote').parent().offset().top - 50 }, 300);
                                $('#summernote').summernote("focus");
                                return !1;
                            }
                            if ($("#needScore").is(":checked")) {
                                if (t.tools.isEmptyObject($("#payScore").val())) {
                                    layer.msg("请设置查看回答所需积分！"); return !1;
                                }
                            }
                            var qid;
                            var _action = location.pathname.split("/")[1];
                            var layerIndex_comment = layer.load("正在提交评论");
                            $.post("/comment/{0}Comment?t={1}".Format(_action, t.getPK()),
                                {
                                    body: encodeURI($('#summernote').summernote("code")),
                                    qid: ((qid = $(".post-permalink").data("mid") || $(".btn_comment").data("mid"))) == location.pathname.split("/")[3] ? qid : 0,
                                    needscore: $("#needScore").is(":checked"),
                                    payscore: $("#payScore").val(),
                                    commentEnum: _action.toUpperCase(),
                                    isanonymous: t.bbs()("isAnonymous")
                                }, function (data) {
                                    if (data.Ok) {
                                        layer.msg("评论成功", {
                                            time: 500, end: function () { location.reload(!0); }
                                        });
                                    } else {
                                        layer.msg(data.Msg || "异常"); return !1;
                                    }
                                    layer.close(layerIndex_comment);
                                }).error(function (data) {
                                    alert(data.Msg || "提交异常，稍后再试!");
                                    layer.close(layerIndex_comment);
                                });
                        })
                    )
            } else {
                //location.href = "/account/login?returnurl={0}".Format(location.href);
                t.gologin();
            }
        }), t;
    },
    LoadPriseControl: function () {
        var t = this, priseflat = !1;
        //私有方法
        function Prise(type, callback) {
            var thisPrise = $(this),
                _action = location.pathname.split("/")[1];
            thisPrise.unbind("click");
            $.post("/comment/{0}prise?t={1}".Format(_action, t.getPK()), {
                objId: (type == 1 ? thisPrise.parent().data("mid") : (thisPrise.data("answer-id"))),
                type: type,
                priseEnum: _action.toUpperCase()
            }, function (data) {
                if (data.Ok) {
                    callback(thisPrise, data);
                } else {
                    layer.msg(data.Msg || "点赞失败");
                    thisPrise.click(function () { Prise.call(this, [type, callback]); });
                }
            }).error(function (data) {
                thisPrise.click(function () { Prise.call(this, [type, callback]); });
                alert(data.Msg || "提交异常，稍后再试!");
            });
        }
        function MainPriseCallBack(thisPrise, data) {
            layer.msg(data.Msg || "点赞成功");
            thisPrise.addClass("disabled").attr("disabled", !0).text("已点赞")
        }

        function CommentPriseCallBack(thisPrise, data) {
            layer.msg(data.Msg || "点赞成功");
            thisPrise.removeClass("add_prise").addClass("disabled").attr("title", "您已赞过").find("img").attr("src", "/Content/img/icon/prised.svg");
            var count = thisPrise.find(".count");
            count.text(parseInt(count.text()) + 1);
        }
        return function () {
            return $(".btn_prise").unbind("click").bind("click", function () {
                Prise.call(this, 1, MainPriseCallBack);
            })
                , $('.add_prise').unbind("click").bind("click", function () {
                    Prise.call(this, 2, CommentPriseCallBack);
                })
                , $(".aw-add-comment").each(function () {
                    //if (!t.user.login) return !1;
                    var thisComment = $(this);
                    thisComment.data("comment-id"),
                        thisComment.unbind("click").click(function () {
                            t.RichText("#summernote_reply", "/tool/uploadimg", "请发挥你的见解", 260, 0, function (data) {
                                $("#summernote_reply").summernote('insertImage', data.Url);
                            }, null, !0)
                                , layer.open({
                                    type: 1,
                                    title: false,
                                    closeBtn: 1,
                                    area: ["500px", "410px"],
                                    shadeClose: !0,//单击遮罩层关闭窗口
                                    content: $("#replyWrap"),
                                    end: function () {
                                        $("#summernote_reply_wrap").empty().append($("<div>").attr("id", "summernote_reply"));
                                    },
                                }),
                                $("#btn_subReply").unbind("click").click(function () {
                                    if ($("#summernote_reply").summernote("isEmpty")) {
                                        layer.msg("请发挥你的见解"); return !1;
                                    }
                                    else {
                                        var _action = location.pathname.split("/")[1];
                                        var layerindexreply = LOAD("正在回复");
                                        $.post("/comment/{0}Reply?t={1}".Format(_action, t.getPK()), {
                                            body: encodeURI($("#summernote_reply").summernote("code")),
                                            mid: thisComment.data("mid"),
                                            replytop: thisComment.data("comment-id"),
                                            reply2u: thisComment.data("replyuid"),
                                            replytoanswer: thisComment.data("replytoanswer"),
                                            commentEnum: _action.toUpperCase()
                                        }, function (data) {
                                            if (data.Ok) {
                                                layer.msg("回复成功", {
                                                    time: 500, end: function () { location.reload(!0); }
                                                });
                                            } else {
                                                layer.msg(data.Msg);
                                            }
                                            CLOSE(layerindexreply);
                                        }).error(function (data) {
                                            alert(data.Msg || "提交异常，稍后再试!");
                                        });
                                    }
                                });
                        });
                }),
                t;
        }();
    },
    InitLikeControlForDiscuz: function (a) {
        var t = this, type = arguments[0], mid = arguments[1];
        function Un() {
            $(".btn_unlike").length > 0 && (
                $(".btn_unlike").unbind("click").click(function () {
                    Like.call(this, 1);
                })
                    .mouseover(function () {
                        $(this).text("取消关注")
                    })
                    .mouseleave(function () {
                        $(this).text("已关注")
                    }),
                $(".btn_like").unbind("click"));
        }
        function L() {
            $(".btn_like").length > 0 && ($(".btn_like").unbind("click").click(function () {
                Like.call(this);
            })
                .mouseover(function () {
                    $(this).text("关注该贴")
                })
                .mouseleave(function () {
                    $(this).text("关注该贴")
                }),
                $(".btn_unlike").unbind("click"));
        }
        function Like() {
            var $this = $(this);
            var f = arguments.length > 0;
            var mid = $this.parent().data("mid") == mid ? mid : $this.parent().data("mid");
            $.post("/bbs/like?t=" + t.getPK(), {
                id: mid, type: type, unlike: f
            }, function (rd) {
                if (rd.Ok) {
                    if (f) {
                        $this.parents("#post_" + mid).find(".btn_unlike").addClass("btn_like").removeClass("btn_unlike").text("关注该贴");
                        //$this.addClass("btn_like").removeClass("btn_unlike").text("关注该贴");
                        L();
                    } else {
                        $this.parents("#post_" + mid).find(".btn_like").addClass("btn_unlike").removeClass("btn_like").text("取消关注");
                        //$this.addClass("btn_unlike").removeClass("btn_like").text("取消关注");
                        Un();
                    }
                    $this.blur();
                    $(".likecount").length && $(".likecount").text(f ? (parseInt($(".likecount").text()) - 1) : (parseInt($(".likecount").text()) + 1))
                    layer.msg(rd.Msg);

                } else {
                    layer.msg(rd.Msg || "失败");
                }
            }).error(function (data) {
                alert(data.Msg || "提交异常，稍后再试!");
            });
        }
        Un(); L();
    },
    LoadCommentControlForDiscuz: function (showRichText) {
        showRichText = showRichText || false;
        var t = this, time = 650, cflag = !1;
        //return $(".btn_comment").unbind("click").click(), t;
        setTimeout(function () {
            var editorShow = function (focus) {
                $("body,html").animate({ scrollTop: $('#summernote').parent().offset().top - 50 }, 200), focus && $('#summernote').summernote("focus");
            }

            //点击评论 滚动到评论区
            $(".btn_comment").UnBindAndBind("click", function () {
                editorShow(!0);
            })

            $(".note-editor").length ? (
                cflag ? (cflag = !1, $(".note-editor,.commentAreaItems").hide(time), this.innerText = "评论",
                    $("#needScore").attr("checked", !1), $("#payScore").attr("disabled", !0).val("")
                ) :
                    (cflag = !0, editorShow(true)))
                : (
                    cflag = !0,
                    //this.innerText = "暂不评论",
                    (t.RichText("#summernote", "/tool/uploadimg", "请发挥你的见解", 300, 0, function (data) {
                        $("#summernote").summernote('insertImage', data.Url);
                    }, function () {
                        //评论回调关闭
                        // showRichText ? editorShow() : $(".note-editor").hide()
                    }, !1, null, !0)),
                    //显示
                    $(".note-editor").show(time, function () {
                        $(".commentAreaItems").removeClass("Ldn");
                    }),
                    $("#needScore").unbind("change").change(function () {
                        $("#payScore").attr("disabled", this.checked ? !1 : !0).val("");
                    }),
                    $(".btn_Answer").unbind("click").click(function () {
                        if (t.user.login) {
                            var _btn_me = $(this);
                            if ($('#summernote').summernote("isEmpty")) {
                                layer.msg("评论内容不能为空");
                                $("body,html").animate({ scrollTop: $('#summernote').parent().offset().top - 50 }, 300);
                                $('#summernote').summernote("focus");
                                return !1;
                            }
                            if ($("#needScore").is(":checked")) {
                                if (t.tools.isEmptyObject($("#payScore").val())) {
                                    layer.msg("请设置查看回答所需积分！"); return !1;
                                }
                            }
                            var qid;
                            var _action = location.pathname.split("/")[1];
                            var layerIndex_comment = layer.load("正在提交评论");
                            $.post("/comment/{0}Comment?t={1}".Format(_action, t.getPK()),
                                {
                                    body: encodeURI($('#summernote').summernote("code")),
                                    qid: ((qid = $(".post-permalink").data("mid") || $(".btn_comment").parent().data("mid"))) == location.pathname.split("/")[3] ? qid : 0,
                                    needscore: $("#needScore").is(":checked"),
                                    payscore: $("#payScore").val(),
                                    commentEnum: _action.toUpperCase(),
                                    isanonymous: t.bbs()("isAnonymous")
                                }, function (data) {
                                    if (data.Ok) {
                                        layer.msg("评论成功", {
                                            time: 500, end: function () { location.reload(!0); }
                                        });
                                    } else {
                                        layer.msg(data.Msg || "异常"); return !1;
                                    }
                                    layer.close(layerIndex_comment);
                                }).error(function (data) {
                                    alert(data.Msg || "提交异常，稍后再试!");
                                    layer.close(layerIndex_comment);
                                });
                        } else {
                            //location.href = "/account/login?returnurl={0}".Format(location.href);
                            t.gologin();
                        }
                    })
                )

        }, time)
        return t;
    },
    LoadPriseControlForDiscuz: function () {
        var t = this, priseflat = !1;
        //私有方法
        function Prise(type, callback) {
            var thisPrise = $(this),
                _action = location.pathname.split("/")[1];
            thisPrise.unbind("click");
            $.post("/comment/{0}prise?t={1}".Format(_action, t.getPK()), {
                objId: (type == 1 ? thisPrise.parent().data("mid") : (thisPrise.data("answer-id"))),
                type: type,
                priseEnum: _action.toUpperCase()
            }, function (data) {
                if (data.Ok) {
                    callback(thisPrise, data);
                } else {
                    layer.msg(data.Msg || "点赞失败");
                    thisPrise.click(function () { Prise.call(this, [type, callback]); });
                }
            }).error(function (data) {
                thisPrise.click(function () { Prise.call(this, [type, callback]); });
                alert(data.Msg || "提交异常，稍后再试!");
            });
        }
        function Against(type, callback) {
            var thisAgainst = $(this),
                _action = location.pathname.split("/")[1];
            thisAgainst.unbind("click");
            $.post("/comment/{0}against?t={1}".Format(_action, t.getPK()), {
                objId: (type == 1 ? thisAgainst.parent().data("mid") : (thisAgainst.data("answer-id"))),
                type: type,
                priseEnum: _action.toUpperCase()
            }, function (data) {
                if (data.Ok) {
                    callback(thisAgainst, data);
                } else {
                    layer.msg(data.Msg || "反对失败");
                    thisAgainst.click(function () { Prise.call(this, [type, callback]); });
                }
            }).error(function (data) {
                thisAgainst.click(function () { Against.call(this, [type, callback]); });
                alert(data.Msg || "提交异常，稍后再试!");
            });
        }
        function MainPriseCallBack(thisPrise, data) {
            layer.msg(data.Msg || "点赞成功");
            var btn = $("#post_" + thisPrise.parent().data("mid")).find(".btn_prise");
            btn.addClass("disabled").attr("disabled", !0).text("已点赞");
            var next = btn.next();
            next.hasClass("VoteButton") && next.removeClass("Ldni");
        }
        function MainAgainstCallBack(thisAgainst, data) {
            layer.msg(data.Msg || "反对成功");
            var btn = $("#post_" + thisAgainst.parent().data("mid")).find(".btn_against");
            btn.addClass("disabled").attr("disabled", !0).text("已反对");
            var next = btn.next();
            next.hasClass("VoteButton") && next.removeClass("Ldni");
        }
        function CommentPriseCallBack(thisPrise, data) {
            layer.msg(data.Msg || "点赞成功");
            var count = thisPrise.find(".count");
            thisPrise.removeClass("add_prise").addClass("disabled").html('已点赞<span class="count">({0})</span>'.Format(parseInt(count.text()) + 1));
            var next = thisPrise.next();
            next.hasClass("VoteButton") && next.removeClass("Ldni");
            count.text();
        }
        function CommentAgainstCallBack(thisAgainst, data) {
            layer.msg(data.Msg || "反对成功");
            var count = thisAgainst.find(".count");
            thisAgainst.removeClass("add_against").addClass("disabled").html('已反对<span class="count">({0})</span>'.Format(parseInt(count.text()) + 1));
            var next = thisAgainst.next();
            next.hasClass("VoteButton") && next.removeClass("Ldni");
            count.text();
        }
        return function () {
            //添加主内容的赞
            $(".btn_prise").unbind("click").bind("click", function () {
                Prise.call(this, 1, MainPriseCallBack);
            });
            //添加评论的赞
            $('.add_prise').unbind("click").bind("click", function () {
                Prise.call(this, 2, CommentPriseCallBack);
            });
            //添加主内容的反对
            $(".btn_against").UnBindAndBind("click", function () {
                Against.call(this, 1, MainAgainstCallBack);
            });
            //添加评论的反对
            $('.add_against').unbind("click").bind("click", function () {
                Against.call(this, 2, CommentAgainstCallBack);
            });
            $(".aw-add-comment").each(function () {
                //if (!t.user.login) return !1;
                var thisComment = $(this);
                thisComment.data("comment-id"),
                    thisComment.unbind("click").click(function () {
                        t.RichText("#summernote_reply", "/tool/uploadimg", "请发挥你的见解", 260, 0, function (data) {
                            $("#summernote_reply").summernote('insertImage', data.Url);
                        }, null, !0)
                            , layer.open({
                                type: 1,
                                title: false,
                                closeBtn: 1,
                                area: ["500px", "410px"],
                                shadeClose: !0,//单击遮罩层关闭窗口
                                content: $("#replyWrap"),
                                end: function () {
                                    $("#summernote_reply_wrap").empty().append($("<div>").attr("id", "summernote_reply"));
                                },
                            }),
                            $("#btn_subReply").unbind("click").click(function () {
                                if ($("#summernote_reply").summernote("isEmpty")) {
                                    layer.msg("请发挥你的见解"); return !1;
                                }
                                else {
                                    var _action = location.pathname.split("/")[1];
                                    var layerindexreply = LOAD("正在回复");
                                    $.post("/comment/{0}Reply?t={1}".Format(_action, t.getPK()), {
                                        body: encodeURI($("#summernote_reply").summernote("code")),
                                        mid: thisComment.data("mid"),
                                        replytop: thisComment.data("comment-id"),
                                        reply2u: thisComment.data("replyuid"),
                                        replytoanswer: thisComment.data("replytoanswer"),
                                        commentEnum: _action.toUpperCase()
                                    }, function (data) {
                                        if (data.Ok) {
                                            layer.msg("回复成功", {
                                                time: 500, end: function () { location.reload(!0); }
                                            });
                                        } else {
                                            layer.msg(data.Msg);
                                        }
                                        CLOSE(layerindexreply);
                                    }).error(function (data) {
                                        alert(data.Msg || "提交异常，稍后再试!");
                                    });
                                }
                            });
                    });
            });
            return t;
        }();
    },
    SendFile: function (url, files, isThnumbnail, callback, onElementClick, fileSize, uploadFileIndex) {
        var me = this;
        function U() {
            fileSize = fileSize || 1;
            //if (!files.files) { layer.msg("请上传附件"); return !1; }
            //var _files = files.length == 1 ? files[0] : files.files[0];
            var _files;
            if (files.type == "file") {
                _files = files.files;
                if (_files.length == 0) {
                    layer.msg("请选择要上传的文件");
                    return !1;
                }
            } else {
                _files = files;
                if (files.length == 0) {
                    layer.msg("请选择要上传的文件");
                    return !1;
                }
            }
            //if (_files[0].size > fileSize * 1024 * 1024) {
            //    layer.msg("最大只能上传{0}M的文件".Format(fileSize));
            //    return 11;
            //}
            var layindex
            if (uploadFileIndex) {
                layindex = layer.load("正在上传第" + uploadFileIndex + "个...");
            } else {
                layindex = layer.load("奋力上传中…");
            }
            var fileCount = _files.length;
            var onUploadSuccessImageSrcs = [];
            var uploadImgFunction = function (fileIndex) {
                var formdata = new FormData();
                var _fileName_ = _files[fileIndex].name;
                //formdata.append("file", $('.note-image-input')[0].files[0]);
                formdata.append("file", _files[fileIndex]);
                formdata.append("IsThumbnail", isThnumbnail);
                $.ajax({
                    data: formdata,
                    type: "POST",
                    url: url,
                    cache: false,
                    contentType: false,
                    processData: false,
                    dataType: "json",
                    success: function (data) {
                        if (!data.Ok) {
                            layer.close(layindex);
                            layer.msg(data.Msg || "上传失败");
                        } else {
                            console.log("上传成功：" + _files[fileIndex].name + "   地址：" + data.Url);
                            onUploadSuccessImageSrcs.push({ imageName: _fileName_, url: data.Url });
                            if (fileIndex < fileCount - 1) {
                                console.log(fileIndex + 1)
                                uploadImgFunction(fileIndex + 1);
                            } else {
                                if (me.tools.isFunction(callback)) {
                                    //$.each(onUploadSuccessImageSrcs, function (i, d) {
                                    //    callback(onUploadSuccessImageSrcs[i], i);
                                    //});
                                    function renderImg(renderIndex) {
                                        callback(data, onUploadSuccessImageSrcs[renderIndex].url, onUploadSuccessImageSrcs[renderIndex].imageName, function () {
                                            console.log("render:" + renderIndex)
                                            if (renderIndex < fileCount - 1) {
                                                renderImg(renderIndex + 1);
                                            } else {
                                                layer.close(layindex);
                                            }
                                        })
                                    }
                                    renderImg(0);
                                    if (uploadFileIndex == undefined || uploadFileIndex == null) {
                                        layer.close(layindex);
                                    }
                                }
                            }
                            //me.tools.isFunction(callback) && callback(data, function () {
                            //    if (fileIndex < fileCount - 1) {
                            //        setTimeout(function () {
                            //            console.log(fileIndex + 1)
                            //            uploadImgFunction(fileIndex + 1);
                            //        }, 500);
                            //    } else {
                            //        layer.close(layindex);
                            //    }
                            //});
                        }
                    },
                    error: function () {
                        layer.close(layindex);
                        layer.msg("上传失败");
                    }
                });
            }
            uploadImgFunction(0);//循环上传
        }
        if (onElementClick) {
            $(onElementClick).unbind("click").bind("click", function () {
                U();
            });
        } else { U(); }
        return this;
    },
    SendFile2: function (url, isThnumbnail, callback, params, files, fileMax) {
        var me = this;
        if (files) {
            var _files;
            if (files.type == "file") {
                _files = files.files;
                if (_files.length == 0) {
                    Leo.msgfail("请选择要上传的文件");
                    return !1;
                }
            } else {
                _files = files;
                if (files.length == 0) {
                    Leo.msgfail("请选择要上传的文件");
                    return !1;
                }
            }
            if (_files[0].size > fileMax * 1024 * 1024) {
                Leo.msgfail("最大只能上传1M的文件");
                return 11;
            }
            params.append("file", _files[0]);
        }
        var layindex = layer.load("奋力上传中…");
        $.ajax({
            data: params,
            type: "POST",
            url: url,
            cache: false,
            contentType: false,
            processData: false,
            dataType: "json",
            success: function (data) {
                layer.close(layindex);
                if (data.Ok) {
                    me.tools.isFunction(callback) && callback(data);
                } else {
                    layer.msg(data.Msg || "失败");
                }
            },
            error: function () {
                layer.close(layindex);
                layer.msg("失败");
            }
        });

    },
    //SummernoteAutoSave: function (mainType, mainId) {
    //    var me = this;
    //    me.summernote_cacheKey = "summernote_";
    //    mainType && (me.summernote_cacheKey += mainType)
    //    mainId && (me.summernote_cacheKey += "_" + mainId)
    //    me.user.login && (me.summernote_cacheKey += "_" + me.user.id)
    //    console.log(me.summernote_cacheKey);
    //    //查看本地有没有存储内容
    //    var content = localStorage.getItem(me.summernote_cacheKey);
    //    if (!me.tools.isEmptyObject(content)) {
    //        console.log("获取到内容");
    //        me.summernote_content || (me.summernote_content = {})
    //        me.summernote_content[me.summernote_cacheKey] = content;
    //    }
    //    return me;
    //},
    //SummernoteAutoSaveType: {
    //    add: {
    //        bbs: "BBS_ADD",
    //        article: "ARTICLE_ADD",
    //        party: "PARTY_ADD",
    //        zhaopin: "ZHAOPIN_ADD",
    //        qiuzhi: "QIUZHI_ADD",
    //    },
    //    comment: {
    //        bbs: "BBS_COMMENT",
    //        article: "ARTICLE_COMMENT",
    //        party: "PARTY_COMMENT",
    //        zhaopin: "ZHAOPIN_COMMENT",
    //        qiuzhi: "QIUZHI_COMMENT",
    //    },
    //},
    RichText: function (richTextElementId, url, richPlaceholder, richHeight, onImageIsNeedThumbnail, callback, oninit, isScroll, LoadContent, nofocus) {
        var me = this;
        return function (richId, isThnumbnail, callback) {
            var $richId = $(richId);
            var onSummernoteInitSuccessCallback = function () {
                me.tools.isFunction(oninit) && oninit();
                //if (me.summernote_cacheKey && me.summernote_content) {
                //    me.msg("富文本区域已还原上次编辑的内容，可以继续编辑或重新编辑")
                //    $richId.summernote("code", me.summernote_content[me.summernote_cacheKey]);
                //}
                //me.summernoteInterval = setInterval(function () {
                //    if (!$richId.summernote("isEmpty")) {
                //        localStorage.setItem(me.summernote_cacheKey || "su", $richId.summernote("code"));
                //    }
                //}, 1000);
            }
            $richId.summernote({
                placeholder: (me.user.login ? richPlaceholder : "您没有登录，请先登录再发表！"),
                tabsize: 2,
                height: richHeight,
                lang: 'zh-CN',
                focus: nofocus ? !nofocus : !0,
                callbacks: {
                    onInit: onSummernoteInitSuccessCallback,
                    onImageUpload: function (files, editor, $editable) {
                        var fileCount = files.length;
                        var findex = 0;
                        var renderInterval = setInterval(function () {
                            //先渲染到富文本里
                            var tempFile = [files[findex]]
                            $richId.summernote("insertImages", tempFile);
                            findex++;
                            //上传服务器
                            isThnumbnail = isThnumbnail ? 1 : 0;
                            //优化 到此处 渲染 图片地址 - 20191217
                            //me.SendFile(url, files, isThnumbnail, callback, null);
                            me.SendFile(url, tempFile, isThnumbnail, function (data, url, imageName, onRenderImageSrcSuccessCallBack) {
                                //$richId.summernote('insertImage', data.Url, function () {
                                //    me.tools.isFunction(onRenderImageSrcSuccessCallBack) && onRenderImageSrcSuccessCallBack();
                                //});
                                var imgEle = $(".note-editable img[data-filename='" + imageName + "']")
                                imgEle.attr({ "src": url, "data-filename": data.Url });
                                //me.tools.isFunction(onRenderImageSrcSuccessCallBack) && onRenderImageSrcSuccessCallBack();
                            }, null, null, findex);
                            if (findex >= fileCount) {
                                clearInterval(renderInterval);
                                layer.closeAll();
                            }
                        }, 1000);
                    },
                    onPaste: function (ne, e) {
                        var pasteFile = ne.originalEvent.clipboardData.files.length;
                        setTimeout(function () {
                            if (pasteFile == 0) {
                                var a = $(richId).summernote("code");
                                var a = a.replace(/style="[^"]*"/g, "").replace(/class="[^"]*"/g, "");
                                $richId.summernote("code", a);
                            }
                        }, 50);
                    }
                },
            });
            //加载评论富文本时进行滚动
            isScroll && setTimeout(function () {
                $("body,html").animate({ scrollTop: $(richId).parent().offset().top - 50 }, 300);
            }, 50),
                LoadContent && $(richId).summernote("code", LoadContent);
        }(richTextElementId, onImageIsNeedThumbnail, callback);
    },
    BT: function () {
        var t;
        var mouseflag = !1;
        var showFlag = !1;
        function GetUserInfo(id, callback) {
            if (t.tools.isString(id) && t.tools.isNumber(parseInt(id))) {
                $.get("/account/GetUser/{0}?t={1}".Format(id, t.getPK()), function (data) {
                    if (data.Ok) {
                        callback(data.Data);
                    }
                });
            }
        }

        function GetContent(data, $this) {
            var div = $("<div>").attr("style", "width: 350px;text-align: center;background-color: #f1f1f1;opacity: 0.8;padding: 10px 20px").addClass("popover_user_{0}".Format(data.UserID));
            var div2 = $("<h6>");
            div2.append(
                $("<img>").attr({ "style": "border-radius:100%;width:48px;height:48px;", "src": "/Content/img/head_default.gif" }));
            //认证
            var vgroup = [];
            switch (data.UserV) {
                case 1: vgroup.push(1); break;
                case 2: vgroup.push(2); break;
                case 4: vgroup.push(4); break;
                case 3: vgroup.push(1, 2); break;
                case 5: vgroup.push(1, 4); break;
                case 6: vgroup.push(2, 4); break;
                case 7: vgroup.push(1, 2, 4); break;
            }
            $.each(vgroup, function (i, n) {
                div2.append(
                    $("<img>").attr({
                        "style": "width:16px;height:16px;vertical-align:bottom !important;", "src": "/Content/U/UserV/{0}.png".Format(n)
                    }));
            })

            var genderImg = "";
            if (data.Gender == 1) {
                genderImg = "<img width='16' height='16' src='/Content/img/man.png'/>";
            } else if (data.Gender == 2) {
                genderImg = "<img width='16' height='16' src='/Content/img/woman.png'/>";
            }

            var div3 = $("<div>").addClass("col-md-12").append('<h5 class="col-md-12">{0}{2}</h5><h6 class="col-md-12">{1}</h6>'.Format(data.UserName, (data.Sign || "该用户很懒，什么都没留下~"), genderImg));
            var div4 = $("<h6>").addClass("col-md-12 clearfix").append('<div class="col-xs-4 col-md-4">文章<span>{0}</span></div><div class="col-xs-4 col-md-4">问题<span>{1}</span></div><div class="col-xs-4 col-md-4">粉丝<span class="pop_likecount">{2}</span></div>'.Format(data.ArticltCount, data.QuestionCount, data.LikeCount));
            var div5 = $("<div>").addClass("col-md-12 clearfix");
            var div51 = $("<div>").addClass("col-xs-6 col-md-6").append(
                $("<a>").addClass("btn btn-primary {0}".Format(data.IsLiked ? "btn_unlike" : "btn_like")).text(data.IsLiked ? "已关注" : "关注")
                    .mouseover(function () {
                        $(this).text(data.IsLiked ? "取消关注" : "关注")
                    })
                    .mouseleave(function () {
                        $(this).text(data.IsLiked ? "已关注" : "关注")
                    })
                    .click(function () {
                        if (t.user.login) {
                            var $this = $(this);
                            var f = $this.hasClass("btn_like") ? !1 : !0;
                            $.post("/bbs/like?t=" + t.getPK(), {
                                id: data.UserID,
                                type: 3,
                                unlike: f
                            }, function (rd) {
                                if (rd.Ok) {
                                    if (f) {
                                        $this.addClass("btn_like").removeClass("btn_unlike").text("关注");
                                    } else {
                                        $this.addClass("btn_unlike").removeClass("btn_like").text("已关注");
                                    }
                                    $(".pop_likecount").text(f ? (parseInt($(".pop_likecount").text()) - 1) : (parseInt($(".pop_likecount").text()) + 1))
                                    layer.msg(rd.Msg);
                                    data.IsLiked = !f;

                                } else {
                                    layer.msg(rd.Msg || "失败");
                                }
                            }).error(function (data) {
                                alert(data.Msg || "提交异常，稍后再试!");
                            });
                        } else {
                            layer.msg("请先登录");
                        }
                    }
                    ));
            var div52 = $("<div>").addClass("col-xs-6 col-md-6").append(
                $("<a>").addClass("btn btn-primary btn_sendMsg").text("私信")
                    .click(function () {
                        t.user.login ? (t.ChatInit(data.UserID, data.UserName)) : (layer.msg("请先登录"));
                    }));
            return div.append(div2, div3, div4, div5.append(div51, div52)).mouseover(function () { mouseflag = !0; }).mouseleave(function (e) {
                e.preventDefault();
                mouseflag = !1;
                $this.popover("destroy");
            });
        }

        return {
            Popover: function () {
                t = this;
                $("[data-toggle='popover']").mouseover(function (e) {
                    showFlag = !0;
                    var $this = $(this);
                    e = e;
                    setTimeout(function () {
                        if (showFlag) {
                            if (e.eventPhase == 3) { return !1; }
                            e.preventDefault();
                            if ($this.attr("href").split("/")[3] != t.user.uname) {
                                if (!$this.next().hasClass("popover")) {
                                    var _user = GetUserInfo($this.attr("href").split("/")[3], function (data) {
                                        //if($(".popover_user_{0}".data))
                                        $this.popover({
                                            //trigger: 'hover',
                                            html: true,
                                            //title: "",
                                            content: GetContent(data, $this)
                                        }).popover("show");
                                    });
                                }
                            }
                        }
                    }, 200);
                }).mouseleave(function (e) {
                    showFlag = !1;
                    e = e;
                    var $this = $(this);
                    setTimeout(function () {
                        if (!mouseflag) {
                            e.preventDefault();
                            $this.popover("destroy");
                        }
                    }, 200);
                });
            }
        }
    }(),
    SearchAllSite: function () {
        var t = this, searchBtn = $(".btn-fullSearch"), sinput = searchBtn.parent().prev().find("input"), select = searchBtn.parent().parent().children().eq(0).children();
        var from = t.tools.QueryString("searchfrom");
        from && select.val(from);
        function Search() {
            var me = $(this);
            if (t.tools.CheckFormNotEmpty(sinput, "搜索内容", !0)) {
                LOAD("正在搜索中");
                location.href = "/s?key={0}&searchfrom={1}".Format(encodeURIComponent(sinput.val()), select.val());
            }
        }
        searchBtn.UnBindAndBind("click", Search);
        t.onfocusKeyup(sinput, t.keyup.onKeyUpType.Enter, Search);
    },
    WeChat: function () {
        var t = this,
            //可聊天状态
            _flat = !1,
            chat = function (toID, toName, onSendMsgSuccessCallback, onSendMsgFailCallback, onCancleCallback) {
                var config = this.config;
                config.to = toID;
                config.name = toName;
                config.batch = t.tools.isArray(toID);
                config.onSuccess = onSendMsgSuccessCallback;
                config.onFail = onSendMsgFailCallback;
                config.onCancleCallback = onCancleCallback;
            };
        chat.prototype.config = {
            to: null,
            name: null,
            message: null,
            time: 5,
            inboxIndex: 0,
            errorMsgIndex: 0,
            sendingIndex: 0,
            flag: !1,
            batch: !1,  //是否批量发送 目前只有管理员可以批量发送
            singleUrl: "/c/talk/{0}",
            batchUrl: "/c/talkbatch",
            onSuccess: null,
            onFail: null,
            onCancleCallback: null,
        };
        chat.prototype.createDom = function () {
            var $this = this,
                _c = $this.config;
            //var _css = "<style>.chatbox {padding: 20px 10px;text-align: center;}.chatbox textarea {width: 100%;padding: 10px;height: 130px;min-height: 120px;max-height: 200px;outline: 0;border: 1px solid #ebebeb;resize: none;}.chatbox a {margin: 20px 0;}</style>";
            var chatDom = $("<div>").attr("id", "chatbox").addClass("chatbox Ldn"), ta, senfFun;
            $("<div>").addClass("col-md-12")
                //.append(_css)
                .append("<h3>发送私信</h3>")
                .append("<h4>To：{0}</h4>".Format(_c.name))
                .append(ta = $("<textarea>").attr("placeholder", "请输入私信内容"))
                .append($("<a>").addClass("btn btn-primary btn_send").text("发送").click(senfFun = function (e) {
                    var _me = $(e);
                    _me.unbind("click");
                    _c.message = ta.val();
                    if (t.tools.isEmptyObject(_c.message.trim())) {
                        _c.errorMsgIndex = layer.msg("私信内容不能为空");
                        ta.focus();
                        _me.bind("click", function () { senfFun(this); })
                        return !1;
                    } else {
                        _c.flag = !0;
                        _c.sendingIndex = layer.load("发送消息中");
                        $this.send();
                    }
                }, function () {
                    senfFun(this);
                })).appendTo(chatDom);
            chatDom.appendTo($("body"));
            setTimeout(function () { ta.focus(); }, 50);
            return this;
        };
        chat.prototype.LoadDom = function () {
            var config = this.config;
            config.inboxIndex = layer.open({
                type: 1,
                title: false,
                closeBtn: 0,
                area: ["300px", "auto"],
                shadeClose: !0,//单击遮罩层关闭窗口
                content: $("#chatbox"),
                end: function () {
                    layer.close(config.errorMsgIndex);
                    $("#chatbox").remove();
                    t.tools.isFunction(config.onCancleCallback) && config.onCancleCallback()
                },
            });
            return this;
        },
            chat.prototype.send = function () {
                var $this = this, $config = $this.config;
                if ($config.flag) {
                    setTimeout(function () {
                        var $url, obj = { message: encodeURI($config.message) };
                        if ($config.batch) {
                            $url = $config.batchUrl;
                            obj.ids = $config.to
                        } else {
                            $url = $config.singleUrl.Format($config.to);
                        }
                        $.post($url, obj, function (data) {
                            if (data.Ok) {
                                t.msgsuccess(data.Msg);
                                t.tools.isFunction($config.onSuccess) && $config.onSuccess()
                            } else {
                                t.msgfail(data.Msg || "发送失败");
                                t.tools.isFunction($config.onFail) && $config.onFail();
                            }
                            layer.close($config.sendingIndex);
                            layer.close($config.inboxIndex);
                        });
                    }, 300);
                }
            };
        return function (toID, toName, onSendMsgSuccessCallback, onSendMsgFailCallback, onCancleCallback) {
            if (!t.tools.isNumber(toID) && t.tools.isEmptyObject(toName)) {
                layer.msg("创建会话失败");
                return !1;
            }
            var c = new chat(toID, toName, onSendMsgSuccessCallback, onSendMsgFailCallback, onCancleCallback);
            c.createDom().LoadDom();
        }
    },
    Chat: function () {
        return !1;
        var t = this;
        var socket;
        if (typeof (WebSocket) == "undefined") {
            alert("您的浏览器不支持WebSocket");
            return;
        }
        socket = new WebSocket("ws://localhost:4145/Chat/ws/zhangsan");
        socket.onopen = function () {
            console.log("已打开连接");
        };
        socket.onmessage = function (msg) {
            console.log(msg.data);
        }

        //关闭事件
        socket.onclose = function () {
            console.log("Socket已关闭");
        };
        //发生了错误事件
        socket.onerror = function () {
            console.log("发生了错误");
        }
        return {
            send: function () {
                socket.send("{0}:你好啊".Format(t.getPK()));
            }
        }
    },
    ChatInit: function (id, name, onSuccess, onFail, onCancle) {
        var t = this;
        if (t.tools.isEmptyObject(id) && t.tools.isEmptyObject(name)) {
            $(".btn_sendMsg").unbind("click").bind("click", function () {
                if (t.user.login) {
                    var me = $(this);
                    id = me.data("id");
                    name = me.data("name");
                    Leo.WeChat()(id, name, onSuccess, onFail, onCancle);
                } else {
                    //location.href = "/account/login?returnurl={0}".Format(location.href);
                    t.gologin();
                }
            });
        } else {
            Leo.WeChat()(id, name, onSuccess, onFail, onCancle);
        }
    },
    ReceiveMessage: function () {
        var onLoadMessageCount = 0;
        var layChatBox;
        var messageInterval = null;
        var textarea = null;
        var t = this;
        var time = 5 * 1000;
        var currentReceiveCount = 0;
        var login = false;
        var getInterval = null;

        function voiceNotice() {
            var i = document.createElement("audio");
            i.src = "/Content/mp3/message.mp3";
            i.play();
        }
        function GET(or) {
            if (login && location.href.indexOf("http://www.baixiaotangtop.com/") > -1) {
                $.ajax({
                    type: "post",
                    async: false,
                    url: "/C/MSG?t={0}".Format(t.getPK()),
                    error: function (data) { },
                    success: function (data) {
                        if (data.Ok) {
                            if (data.Data.Chats.length > 0) {
                                if ($(".layercss").length == 0) {
                                    $("body").append('<link class="layercss" href="/Content/layim/layim.css?t={0}" rel="stylesheet" />'.Format(t.getPK()))
                                        .append('<link class="layercss" href="/Content/layim/layui.css?t={0}" rel="stylesheet" />'.Format(t.getPK()))
                                }
                                var allUnRead = 0;
                                var wrap = $(".nav_msg_bar");
                                var ul = wrap.children().eq(-1);
                                ul = ul.hasClass("dropdown-menu") ? ul : $("<ul>").addClass("dropdown-menu dropdown-toggle msgnoticeBox");
                                data.Data.Chats.forEach(function (i, n) {
                                    allUnRead += parseInt(i.UnReadCount);
                                    var curli;
                                    if ((curli = ul.find(".chat_u_{0}".Format(i.FromID))).length > 0) {
                                        if (curli.children().find("span").text() != i.UnReadCount) {
                                            curli.children().html(i.FromUserName).append($("<span>").text(i["UnReadCount"]).addClass("badge pull-right"));
                                        }
                                    } else {
                                        var _li;
                                        ul.append(
                                            (_li = $("<li>"))
                                                .append(
                                                    $("<a>").click(function () {
                                                        if (!!layChatBox) {
                                                            layer.close(layChatBox);
                                                        }
                                                        OnMessage(i, function () {
                                                            var recount = parseInt($(".msgcount").text()) - currentReceiveCount;
                                                            if (!isNaN(recount)) {
                                                                if (recount == 0) {
                                                                    $(".chat_u_{0}".Format(i.FromID)).parent("ul").remove();
                                                                    $(".msgcount").removeClass("badge").text("");
                                                                    //$(".msgbox").addClass("Ldni");
                                                                } else {
                                                                    $(".msgcount").text(recount);
                                                                    //$(".msgbox").removeClass("Ldni");
                                                                    _li.remove();
                                                                }
                                                            }
                                                        }, !1);
                                                        $(document).unbind("keyup").keyup(function (e) {
                                                            if (event.ctrlKey && event.keyCode == 13) {
                                                                Reply(i.FromID);
                                                            } else if (e.keyCode == 27) {
                                                                if (!!layChatBox) {
                                                                    layer.close(layChatBox);
                                                                    StopIntervalMessage();
                                                                }
                                                            }
                                                        });
                                                    }).text(i["FromUserName"])
                                                        .append($("<span>").text(i["UnReadCount"]).addClass("badge pull-right"))
                                                ).addClass("chat_u_{0}".Format(i.FromID)));
                                    }
                                });
                                if (onLoadMessageCount != allUnRead) {
                                    voiceNotice();
                                }
                                onLoadMessageCount = allUnRead;
                                //if (allUnRead > 0) {
                                //    $(".msgbox").removeClass("Ldni");
                                //} else {
                                //    $(".msgbox").addClass("Ldni");
                                //}
                                var allcount = $(".msgcount");
                                allcount.text(allUnRead)
                                allcount.hasClass("badge") || allcount.addClass("badge");
                                wrap.append(ul);
                            }
                            if (data.Data.Notices > 0) {
                                if ($(".layercss").length == 0) {
                                    $("body").append('<link class="layercss" href="/Content/layim/layim.css?t={0}" rel="stylesheet" />'.Format(t.getPK()))
                                        .append('<link class="layercss" href="/Content/layim/layui.css?t={0}" rel="stylesheet" />'.Format(t.getPK()))
                                }
                                var wrap = $(".nav_notice_bar");
                                var ul = wrap.children().eq(-1);
                                ul.find(".noticecount").text(data.Data.Notices);
                                //wrap.find(" .bxtnoticebox").removeClass("Ldni");
                            } else {
                                //$(".nav_notice_bar .bxtnoticebox").addClass("Ldni");
                            }
                        } else {
                            if (data.ID == -9999) {
                                login = false;
                                clearInterval(getInterval);
                            }
                        }
                    }
                });
            }
        }
        function OnMessage(i, callback, isChating) {
            $.get("/C/Listen/{0}?t={1}".Format(i.FromID, t.getPK()), function (data) {
                if (data.Ok) {
                    if (isChating) {
                        voiceNotice();
                    }
                    currentReceiveCount = data.Data.length;
                    BuildDOm(data.Data);
                }
            });
            function BuildDOm(data) {
                if ($("#layui-layim-chat").length == 0) {
                    var FromID = data[0].FromID;
                    var headUrl = data[0].Head || "/Content/img/head_default.gif";
                    var FromUserName = data[0].FromUserName;
                    var Sign = data[0].Sign || "该用户很懒，什么都没有留下~";

                    var chatbox = $("<div>").addClass("layim-chat-box");
                    var chatbox_div = $("<div>").addClass("layim-chat layim-chat-friend layui-show");
                    var chatbox_title = $("<div>").addClass("layui-unselect layim-chat-title");
                    var chatbox_title_other = $("<div>").addClass("layim-chat-other");
                    var chatbox_title_other_img = $("<img>").addClass("layim-friend{0}".Format(FromID)).attr("src", headUrl);
                    var chatbox_title_other_username = $("<span>").addClass("layim-chat-username").text(FromUserName);
                    var chatbox_title_other_sign = $("<p>").addClass("layim-chat-status").append($("<span>").text(Sign).attr("title", Sign)).css({ 'line-height': "24px", "height": "24px", "overflow": "hidden" });

                    var chatbox_main = $("<div>").addClass("layim-chat-main");
                    //var chatbox_mail_history = $('<div class="layim-chat-system"><span layim-event="chatLog">查看更多记录</span></div>');
                    //var chatbox_mail_history = $("<div>").addClass("layim-chat-system").append($("<span>").text("查看更多记录"));
                    var chatbox_main_ul = $("<ul>").addClass("layim-chat-mail-ul");

                    var _msgids = AppendMessage(chatbox_main_ul, data, !1);
                    //li:class="layim-chat-mine"

                    var chatbox_foot = $("<div>").addClass("layim-chat-footer");
                    var chatbox_foot_tool = $("<div>").addClass("layui-unselect layim-chat-tool").append($('<span class="layim-tool-log" layim-event="chatLog">聊天记录</span>')).css("border-bottom", "1px solid #F1F1F1");
                    var chatbox_foot_textarea = $("<div>").addClass("layim-chat-textarea").append(
                        textarea = $("<textarea>").attr({ "style": "height:101px !important;", "placeholder": "在此输入回复内容" }));
                    var chatbox_foot_bottom = $("<div>").addClass("layim-chat-bottom");
                    var chatbox_foot_bottom_send = $("<div>").addClass("layim-chat-send");
                    var chatbox_foot_bottom_send_close = $("<span>").addClass("layim-send-close").html("关闭(Esc)").click(function () {
                        StopIntervalMessage();
                        $("#chatbox").remove();
                        onLoadMessageCount = 0;
                        layer.close(layChatBox);
                    });
                    var chatbox_foot_bottom_send_btn = $("<span>").addClass("layim-send-btn").html("发送(Ctrl+Enter)").click(function () {
                        Reply(FromID);
                    });

                    chatbox_foot_bottom_send.append(chatbox_foot_bottom_send_close).append(chatbox_foot_bottom_send_btn);
                    chatbox_foot_bottom.append(chatbox_foot_bottom_send);
                    chatbox_foot.append(/*chatbox_foot_tool,*/ chatbox_foot_textarea, chatbox_foot_bottom);
                    chatbox_main.append(/*chatbox_mail_history,*/ chatbox_main_ul)
                    chatbox_div.append(chatbox_title.append(chatbox_title_other.append(chatbox_title_other_img).append(chatbox_title_other_username).append(chatbox_title_other_sign))).append(chatbox_main).append(chatbox_foot);
                    chatbox.append(chatbox_div)

                    var _topTitle = $("<div>").addClass("layui-layer-title-chat").css("cursor", "move");
                    $("<div>").attr("id", "chatbox").addClass("Ldn").append(_topTitle, chatbox).appendTo($("body"));
                    layChatBox = layer.open({
                        type: 1,
                        title: !1,
                        closeBtn: 0,
                        area: ["600px", "520px"],
                        shadeClose: !1,//单击遮罩层关闭窗口
                        content: $("#chatbox"),
                        id: "layui-layim-chat",
                        end: function () {
                            $("#chatbox").remove();
                            StopIntervalMessage();
                            onLoadMessageCount = 0;
                            layer.close(layChatBox);
                            $("#layui-layim-chat").remove();
                        },
                        resize: !1,
                        success: function () {
                            //设置已读
                            Read(_msgids);
                            messageInterval = setInterval(function () {
                                OnMessage(i, function (msgid) {
                                    Read(msgid);
                                }, !0);
                            }, time);
                            function Read(msgid) {
                                $.post("/C/Read?t={0}".Format(t.getPK()), {
                                    msgid: msgid.join(",")
                                }, function (data) {
                                    if (data.Ok) {
                                        t.tools.isFunction(callback) && callback();
                                    }
                                });
                            }
                        }
                    });
                } else {
                    var _msgids = AppendMessage($(".layim-chat-mail-ul"), data, !1);
                    t.tools.isFunction(callback) && callback(_msgids);
                }
                $(".layim-chat-main").scrollTop($(".layim-chat-main")[0].scrollHeight + 1e3);
            }
        }
        function Reply(FromID) {
            if (textarea.val() == "") {
                textarea.focus();
                layer.msg("回复内容不能为空"); return !1;
            } else {
                var _textareaValue = textarea.val();
                textarea.val("");
                $.post("/C/Talk/{0}".Format(FromID), {
                    message: encodeURI(_textareaValue)
                }, function (data) {
                    if (data.Ok) {
                        AppendMessage($(".layim-chat-mail-ul"), [data.Data], !0);
                        GOTOCurrentMessage();
                    } else {
                        t.msgfail(data.Msg || "消息未发送成功");
                    }
                });
            }
        }
        function GOTOCurrentMessage() {
            $(".layim-chat-main").scrollTop($(".layim-chat-main")[0].scrollHeight + 1e3);
        }
        function AppendMessage(wrap, data, isMe) {
            var _msgids = [];
            $.each(data, function (i, n) {
                isMe || _msgids.push(n.ChatID);
                var FromUserName = isMe ? "我" : n.FromUserName;
                var headUrl = n.Head || "/Content/img/head_default.gif";
                var stime = isMe ? (t.tools.GetTime()) : (t.tools.ChangeDateFormat(n.SendTime) || t.tools.GetTime());
                var msg = n.Message;
                var chatbox_main_ul_li = $("<li>");
                isMe && chatbox_main_ul_li.addClass("layim-chat-mine");
                var chatbox_main_ul_li_user = $("<div>").addClass("layim-chat-user");
                var chatbox_main_ul_li_user_img = $("<img>").attr("src", headUrl);
                var chatbox_main_ul_li_user_time = $("<cite>{0}<i>{1}</i>{2}</cite>".Format(isMe ? "" : FromUserName, stime, isMe ? FromUserName : ""));
                var chatbox_main_ul_li_text = $("<div>").addClass("layim-chat-text").text(msg);
                chatbox_main_ul_li_user.append(chatbox_main_ul_li_user_img, chatbox_main_ul_li_user_time)
                wrap.append(chatbox_main_ul_li.append(chatbox_main_ul_li_user).append(chatbox_main_ul_li_text));
            });
            return _msgids;
        }
        function StopIntervalMessage() {
            if (messageInterval != null) {
                clearInterval(messageInterval);
                messageInterval = null;
            }
        }
        function Start() {
            $.get("/account/ChecnLogin?t={0}".Format(t.getPK()), function (data) {
                if (data.Ok) {
                    login = true;
                    GET();
                    getInterval = setInterval(GET, time);
                }
            });
        }
        Start();
    },
    account: function () {
        var t = this,
            check = t.tools.CheckFormNotEmpty,
            T = {
                register: function () {
                    var istest = false;
                    if (istest) {
                        ////以下为测试
                        $("#Account").val("11").attr("readonly", !0);
                        t.SendCode.init($("#SendEmailCode"));
                        $("#btn_register").unbind("click").click(function () {
                            if (check($("#ValidateCode"), "邮箱验证码不能为空")) {
                                if (check($("#Password"), "密码不能为空")) {
                                    if ($("#Password").val().length >= 6) {
                                        if ($("#Password").val().length <= 18) {
                                            //if (check($("#UserName"), "昵称不能为空")) {
                                            //if (check($("#Gender"), "请选择性别")) {
                                            //if (check($("#Province"), "请选择省份")) {
                                            //if (check($("#Birth"), "出生年月不能为空")) {
                                            //if (check($("#Work"), "岗位不能为空")) {
                                            $("#nmform").ajaxSubmit(function (data) {
                                                if (data.Ok) {
                                                    location.href = "/";
                                                } else {
                                                    layer.msg(data.Msg || "注册错误"); return !1;
                                                }
                                            }, !0);
                                            //                }
                                            //            }
                                            //        }
                                            //    }
                                            //}
                                        } else {
                                            $("#Password").focus(); layer.msg("密码长度不能大于18位"); return !1;
                                        }
                                    } else {
                                        $("#Password").focus(); layer.msg("密码长度不能小于6位"); return !1;
                                    }
                                }
                            }
                        });
                        ////以上为测试
                    }
                    else {
                        $("#Account").focus();
                        $("#SendEmailCode").click(function () {
                            if (check("#Account", "用户名不能为空")) {
                                var _ = $(this);
                                _.text("发送邮件中");
                                $.post("/tool/RegistEmail?t=" + t.getPK(), {
                                    mail: $.trim($("#Account").val()),
                                    from: location.pathname.split("/")[2].toLowerCase() == "registoauth"
                                }, function (data) {
                                    if (data.Ok) {
                                        $("#Account").attr("readonly", !0);
                                        t.SendCode.init($("#SendEmailCode"));
                                        $("#btn_register").unbind("click").click(function () {
                                            if (check("#ValidateCode", "邮箱验证码不能为空")) {
                                                if (check("#Password", "密码不能为空")) {
                                                    if ($("#Password").val().length >= 6) {
                                                        if ($("#Password").val().length <= 18) {
                                                            //if (check("#UserName", "昵称不能为空")) {
                                                            //    if (check("#Gender", "请选择性别")) {
                                                            //        if (check("#Province", "请选择省份")) {
                                                            //            if (check("#Birth", "出生年月不能为空")) {
                                                            //                if (check("#Work", "岗位不能为空")) {
                                                            var registerIndex = LOAD("注册中");
                                                            $("#nmform").ajaxSubmit(function (data) {
                                                                if (data.Ok) {
                                                                    location.href = data.Url || "/";
                                                                } else {
                                                                    CLOSE(registerIndex);
                                                                    layer.msg(data.Msg || "注册错误");
                                                                    return !1;
                                                                }
                                                            }, !0);
                                                            //                }
                                                            //            }
                                                            //        }
                                                            //    }
                                                            //}
                                                        } else {
                                                            $("#Password").focus();
                                                            layer.msg("密码长度不能大于18位");
                                                            return !1;
                                                        }
                                                    } else {
                                                        $("#Password").focus();
                                                        layer.msg("密码长度不能小于6位");
                                                        return !1;
                                                    }
                                                }
                                            }
                                        });
                                    }
                                    else {
                                        _.text("获取邮箱验证码");
                                        $("#Account").val("").focus();
                                        layer.msg(data.Msg);
                                    }
                                }).error(function (data) {
                                    $(this).text("获取邮箱验证码");
                                });
                            }
                        });
                    }
                    $("#UserName").blur(function () {
                        $("#UserName").val($.trim($("#UserName").val()));
                        if (t.tools.CheckFormNotEmpty($("#UserName"), "昵称")) {
                            $.post("/account/ExistNickName", { nickname: $.trim($("#UserName").val()) }, function (data) {
                                if (data.toLowerCase() == "true") {
                                    layer.msg("您的昵称已有人使用，请重新更换一个");
                                    $("#UserName").focus();
                                }
                            });
                        }
                    });
                    laydate.render({
                        elem: "#Birth",
                        trigger: 'click',
                        type: 'date',
                        min: "1950-01-01",
                        max: "{0}-12-31".Format(new Date().getFullYear() - 5),
                        calendar: !0,
                        showBottom: !1,
                    });
                },
                login: function (objs) {
                    $("#Account").focus();
                    function Logon() {
                        if (check("#Account", "用户名不能为空！")) {
                            if (check("#Password", "密码不能为空！")) {
                                $("#nmform").ajaxSubmit(function (data) {
                                    if (!data.Ok) {
                                        layer.msg(data.Msg || "登录失败！");
                                    } else {
                                        if (data.Url) {
                                            var url = data.Url == "/" ? "/" : data.Url.indexOf("http") > -1 ? data.Url : "http://{0}".Format(data.Url);
                                            location.href = url;
                                        }
                                    }
                                }, !0);
                            }
                        }
                    }
                    $("#btn_logoin").unbind("click").click(function () {
                        Logon();
                    });
                    $(document).keydown(function (e) {
                        if (e.keyCode == 13) {
                            Logon();
                        }
                    });
                    QC && QC.Login({
                        //btnId：插入按钮的节点id，必选
                        btnId: "qqLoginBtn",
                        //用户需要确认的scope授权项，可选，默认all
                        scope: "all",
                        //showModal: true,
                        //按钮尺寸，可用值[A_XL| A_L| A_M| A_S|  B_M| B_S| C_S]，可选，默认B_S
                        size: "A_M"
                    }, function (data, opts) {
                        if (QC.Login.check()) {//如果已登录
                            LOAD('登录中')
                            QC.Login.getMe(function (openId, accessToken) {
                                //alert(["当前登录用户的", "openId为：" + openId, "accessToken为：" + accessToken].join("\n"));
                                var paras = {};
                                QC.api("get_user_info", paras)
                                    .success(function (s) {//完成请求回调
                                        //alert("获取用户信息完成！");
                                        if (data.nickname === s.data.nickname) {
                                            if (QC.Login.check()) {
                                                QC.Login.signOut();
                                            }
                                            //完成回调授权
                                            $.post("/account/QQAuth", { id: openId, token: accessToken, name: s.data.nickname, headurl: s.data.figureurl_qq_2 || s.data.figureurl_1 }, function (data) {
                                                if (!data.Ok) {
                                                    layer.msg(data.Msg || "QQ回调异常");
                                                } else {
                                                    location.href = decodeURIComponent(data.Url || (location.search.replace("?returnurl=", "") || "/"));
                                                    //location.href = location.search.replace("?returnurl=", "") || "/";
                                                }
                                            });
                                        }
                                    }).error(function (f) {
                                        lsyer.msg("获取用户信息失败！");
                                    });
                            });
                        }
                    }, function (opts) {
                    });

                    //手机浏览器上移除微信登录
                    var browser = t.browser;
                    if (browser.versions.ios || browser.versions.android || browser.versions.iPhone || browser.versions.iPad) {
                        $("#wxLoginBtn").hide();
                    }

                    //$("#wxLoginBtn").click(function () {
                    //    $.get("/account/WeChat", function (data) {
                    //        var obj = new WxLogin({
                    //            self_redirect: false,
                    //            id: "login_container",
                    //            appid: objs["wxappid"],
                    //            scope: "snsapi_login",
                    //            redirect_uri: objs["wxcallback"] + location.search,
                    //            state: data,
                    //            style: "",
                    //            href: "wechat_redirect"
                    //        });
                    //        layer.open({
                    //            type: 1,
                    //            title: false,
                    //            closeBtn: 0,
                    //            shadeClose: true,
                    //            skin: 'yourclass',
                    //            content: $("#login_container"),
                    //            success: function (layero, index) {
                    //                var body = layer.getChildFrame('body', index);
                    //                var iframeWin = window[layero.find('iframe')[0]]; //得到iframe页的窗口对象，执行iframe页的方法：iframeWin.method();
                    //                console.log(body.html()) //得到iframe页的body内容
                    //                body.find('input').val('Hi，我是从父页来的')
                    //            }
                    //        });
                    //    });
                    //});
                },
                forget: function () {
                    $("#btn-findpwd").unbind("click").click(function () {
                        if (check("#Account", "请输入邮箱，进行密码找回")) {
                            if (check("#vaCode", "请输入图形验证码")) {
                                $.post("/account/Find", { account: $("#Account").val(), code: $("#vaCode").val() }, function (data) {
                                    if (data.Ok) {
                                        layer.msg('密码已发送到{0}，请注意查收'.Format($("#Account").val()), function () {
                                            location.href = "/account/login";
                                        });
                                    } else {
                                        layer.msg(data.Msg);
                                        if (data.ID == 1) {
                                            $("#Account").focus().val("");
                                        } else if (data.ID == 2) {
                                            var _code = $("#vaCode");
                                            _code.focus();
                                            _code.val("");
                                            $(".vaCode").attr("src", "/tool/VerificationImage?t={0}".Format(t.getPK()));
                                        }
                                    }
                                });
                            }
                        }
                    });
                },
                exchange: function () {
                    $("#btn_exchange").unbind("click").click(function () {
                        if (check("#Account", "用户名")) {
                            if (check("#Password", "旧密码")) {
                                if ($("#Password").val().length >= 6) {
                                    if ($("#Password").val().length <= 18) {
                                        if (check("#newpwd", "新密码")) {
                                            if ($("#newpwd").val().length >= 6) {
                                                if ($("#newpwd").val().length <= 18) {
                                                    if (check("#newpwd2", "重复新密码")) {
                                                        if ($("#newpwd").val() == $("#newpwd2").val()) {
                                                            if (check("#vaCode", "图形验证码")) {
                                                                $.post("/account/ExChange", {
                                                                    account: $("#Account").val(),
                                                                    password: $("#Password").val(),
                                                                    newpwd1: $("#newpwd").val(),
                                                                    newpwd2: $("#newpwd2").val(),
                                                                    code: $("#vaCode").val()
                                                                }, function (data) {
                                                                    if (data.Ok) {
                                                                        layer.msg("密码修改成功", {
                                                                            time: 500, end: function () { location.href = "/account/login"; }
                                                                        });
                                                                    } else {
                                                                        if (data.ID == 1) {
                                                                            layer.msg("用户名不存在");
                                                                            $("#Account").focus().val("");
                                                                        } else if (data.ID == 2) {
                                                                            layer.msg("旧密码错误");
                                                                            $("#Password").focus().val("");
                                                                        } else if (data.ID == 3) {
                                                                            layer.msg("图形验证码错误");
                                                                        } else {
                                                                            layer.msg(data.Msg);
                                                                        }
                                                                        $(".vaCode").attr("src", "/tool/VerificationExchangeImage?t={0}".Format(t.getPK()));
                                                                        var _code = $("#vaCode");
                                                                        _code.focus();
                                                                        _code.val("");
                                                                    }
                                                                });
                                                            }
                                                        } else {
                                                            layer.msg("再次新密码不一致");
                                                        }
                                                    }
                                                } else {
                                                    $("#newpwd").focus(); layer.msg("新密码长度不能大于18位"); return !1;
                                                }
                                            } else {
                                                $("#newpwd").focus(); layer.msg("新密码长度不能小于6位"); return !1;
                                            }
                                        }
                                    } else {
                                        $("#Password").focus(); layer.msg("密码长度不能大于18位"); return !1;
                                    }
                                } else {
                                    $("#Password").focus(); layer.msg("密码长度不能小于6位"); return !1;
                                }
                            }
                        }
                    });
                },
            }
        return function (action, p) {
            return T[action](p);
        }
    },
    home: function () {
        var t = this;
        //签到
        if (!this.user.sign) {
            $("#btn_sign").unbind("click").bind("click", function () {
                $.post("/account/Signture?t=" + t.getPK(), {
                    thunpx: t.user.id
                }, function (data) {
                    if (data.Ok || data.Type == 1) {
                        //$("#btn_sign").unbind("click").removeAttr("id").attr("disabled", !0).find("span").text("今日已签到");
                        $("#btn_sign").unbind("click").remove();
                        $(".btn-signed").removeClass("Ldn");
                        t.MsgTips("签到成功提醒", "您当前总积分：" + data.ID + "<a target='_blank' href='{0}/gift'>兑换</a><br>".Format(t.baseUrl) + (data.Data || "签到成功"));
                    } else {
                        AlertDivNoTitle("", "60%", "50%")
                        layer.msg(data.Msg);
                    }
                });
            });
        }

        //文章和帖子 的 排序图标变更
        function SortImgBtn() {
            var selectTab = $(".userTabs").find(".active");
            var tab = selectTab.children().data("reactid");
            if (tab == "Sort_AllBBS" || tab == "Sort_AllArticle") {
                var imgbtn = selectTab.children().children();
                if (imgbtn.data("sort") == 1) {
                    imgbtn.data("sort", 2).attr("title", "点击按时间升序").removeClass("turnRound");
                } else {
                    imgbtn.data("sort", 1).attr("title", "点击按时间降序").addClass("turnRound");
                }
            } else {
                $("#sort_bbs_icon,#sort_art_icon").css("display", "none");
            }
        }

        var tabsWrapElement = ".userTabs", rendDataElement = ".homeqlist", btnPageElement = "#homecommentpage", url = "/Home/HOMEBBS?sort={0}", loadUrlParams = "Sort_Default", loadPosition_Margin = "-16px 0", loadPosition_Top = "15px";

        t.BootStrap_Tab_Change(tabsWrapElement, rendDataElement, btnPageElement, url, loadUrlParams, loadPosition_Margin, loadPosition_Top, function () {
            buy();
        }, function () {
            SortImgBtn();
        });
        $("#sort_bbs_icon,#sort_art_icon").UnBindAndBind("click", function (e) {
            var me = $(this);
            var activeTab = me.parent().data("reactid");
            var sort = me.data("sort");
            var layerIndex = t.LoadMask(rendDataElement, "-16px 0", "15px");
            var url2 = "/Home/HOMEBBS?sort={0}&order={1}";
            t.ScrollTop(".userTabs");
            //setTimeout(function () {
            $.get("{0}&t={1}".Format(url2.Format(activeTab, sort), t.getPK()), function (data) {
                $(rendDataElement).empty().append(data);
                t.ToolTip();
                t.Page.PageRowNumber(btnPageElement, rendDataElement, "{0}{2}t={1}".Format(url2.Format(activeTab, sort), t.getPK(), url.indexOf("?") > -1 ? "&" : "?"), tabsWrapElement, function () {
                    return t.LoadMask(rendDataElement, "-16px 0", "15px");
                }, sort);
                t.LoadMask.Remove(layerIndex);
                buy();
            }).error(function () { t.LoadMask.Remove(layerIndex); });
            SortImgBtn();
            //}, t.tools.Random(500, 1000));
        });
        $(".showHeadImg").UnBindAndBind("click", function () {
            var me = $(this);
            me.parent().prev().find(".UserHeadInfo").removeClass("Ldn");
            $(this).addClass("Ldn");
        });

        //加载学习更多
        $(".studyinfoMore").click(function () {
            var me = $(this);
            $.get("/home/studyuserinfos", function (res) {
                var p = me.parent();
                p.after(res);
                p.remove();
            });
        });

        function buy() {
            //购买other版块
            $(".btn-buyOtherItem").UnBindAndBind("click", function () {
                if (t.user.login) {
                    var me = $(this);
                    AlertConfirm("确定要花{0}积分查看吗？".Format(me.data("c")), '确定', '取消', function () {
                        $.post("/bbs/feeseeother/{0}".Format(me.data("id")), function (res) {
                            if (res.Ok) {
                                t.msgsuccess(res.Msg || "付费成功");
                                location.href = res.Data
                            } else {
                                t.msgfail(res.Msg || "失败");
                            }
                        });
                    });
                } else {
                    t.gologin();
                }
            });
        }

        /**滚屏公告 */
        function scrollNoticeInit() {
            var wrapper = $(".scrollNoticeWrapper");
            if (wrapper.length > 0) {
                var items = wrapper.find(".scrollNoticeItem");
                if (items.length > 1) {
                    var time = wrapper.data("time") || 3000,
                        count = items.length - 1,
                        currentIndex = 0,
                        $eles = [],
                        stopScrollNotice = false;
                    $.each(items, function (i, ele) {
                        $eles.push($(this));
                    });
                    items.mouseover(function () { stopScrollNotice = true; })
                        .mouseleave(function () { stopScrollNotice = false; });
                    setInterval(function () {
                        if (!stopScrollNotice) {
                            var $thisIndex = currentIndex >= count ? 0 : currentIndex + 1;
                            $eles[currentIndex].addClass("scrollNoticeHide").removeClass("scrollNoticeShow");
                            $eles[$thisIndex].removeClass("scrollNoticeHide").addClass("scrollNoticeShow");
                            currentIndex = $thisIndex;
                        }
                    }, time);
                }
            }
        }

        buy();
        scrollNoticeInit();
    },
    user: function () {
        var t = this;
        var T = {
            edit: function () {
                //更改签名
                $("#btn_MySign").unbind("click").click(function () {
                    if (t.tools.isEmptyObject($("#signinput").val())) {
                        layer.msg("个性签名不能为空!"); return !1;
                    } else {
                        $.post("/User/Sign", {
                            sign: $("#signinput").val()
                        }, function (data) {
                            layer.msg(data.Msg);
                        })
                    }
                });

                //更改昵称
                $("#btn_myNickName").unbind("click").bind("click", function () {
                    var nickname = "#myNickName";
                    nickname.val($.trim(nickname.val()));
                    if ($.trim($(nickname).data("default")) == $.trim(nickname.val())) {
                        layer.msg("您没有修改昵称，无需提交修改");
                        return !1;
                    }
                    if (t.tools.CheckFormNotEmpty(nickname, "昵称")) {
                        var layerindex = layer.load("正在修改昵称");
                        $.post("/User/ChangeNickName", { nickname: nickname.val() }, function (data) {
                            if (data.Ok) {
                                layer.msg("昵称更换成功，一个自然月内只能更换一次！");
                            } else {
                                layer.msg(data.Msg || "更换失败");
                            }
                            layer.close(layerindex);
                        });
                    }
                }).dblclick(function () { });

                //切换头衔显示
                $(".btn-change-showHeadName").click(function () {
                    var me = $(this), id = me.data("id");
                    $.post("/User/SetHeadNameShow/{0}".Format(id), function (data) {
                        if (data.Ok) {
                            layer.msg("切换成功", { time: 300, end: function () { location.reload(!0); } });
                        } else {
                            layer.msg(data.Msg);
                        }
                    });
                });

                //显示个人资料
                $(".btn-change-showHideInfo").click(function () {
                    var me = $(this), value = me.data("value");
                    $.post("/User/SetInfoHideOrShow", { value: value }, function (data) {
                        if (data.Ok) {
                            layer.msg(data.Msg, { time: 300, end: function () { location.reload(!0); } });
                        } else {
                            layer.msg(data.Msg);
                        }
                    });
                });

                //更改性别
                $("#btn_setGender").unbind("click").click(function () {
                    var genderSelect = "#setGender";
                    if (genderSelect.val() == $(genderSelect).data("default")) {
                        layer.msg("性别没有变化，无需更改");
                        return !1;
                    }
                    if (t.tools.CheckFormNotEmpty(genderSelect, "请选择性别")) {
                        var layerIndex = LOAD("正在更换性别，请稍后");
                        $.post("/User/SetGender", { gender: genderSelect.val() }, function (data) {
                            if (data.Ok) {
                                layer.msg("性别更换成功");
                            } else {
                                layer.msg(data.Msg || "性别更换失败");
                            }
                            layer.close(layerIndex);
                        });
                    }
                });

                //更改年龄
                $("#btn_setBirth").unbind("click").click(function () {
                    var birth = "#setBirth";
                    if (birth.val() == $(birth).data("default")) {
                        layer.msg("出生年月日没有变化，无需更改");
                        return !1;
                    }
                    if (t.tools.CheckFormNotEmpty(birth, "请设置出生年月日")) {
                        var layerIndex = LOAD("正在更换出生年月日，请稍后");
                        $.post("/User/SetBirth", { birth: birth.val() }, function (data) {
                            if (data.Ok) {
                                layer.msg("出生年月日更换成功");
                            }
                            else {
                                layer.msg(data.Msg || "出生年月日更换失败");
                            }
                            layer.close(layerIndex);
                        });
                    }
                });

                //更改所在地
                $("#btn_setAreas").unbind("click").click(function () {
                    var province = "#setProvince", city = "#setCity", county = "#setCounty";
                    if (province.val() == $(province).data("default") && city.val() == ($(city).data("default") || "") && county.val() == ($(county).data("default") || "")) {
                        layer.msg("省、市、县都没有变化，无需更改");
                        return !1;
                    }
                    if (t.tools.CheckFormNotEmpty(province, "请设置所在省份，请稍后")) {
                        var layerIndex = LOAD("正在更换所在地");
                        var obj = {};
                        obj["province"] = province.val();
                        city.val() && (obj["city"] = city.val());
                        county.val() && (obj["county"] = county.val());
                        $.post("/User/SetAreas", obj, function (data) {
                            if (data.Ok) {
                                layer.msg("所在地更换成功");
                                $(province).data("default", province.val());
                                $(city).data("default", city.val());
                                $(county).data("default", province.val());
                            }
                            else {
                                layer.msg(data.Msg || "所在地更换失败");
                            }
                            layer.close(layerIndex);
                        });
                    }
                });

                //设置经营类目
                $("#btn_myJingYing").UnBindAndBind("click", function () {
                    if (t.tools.isEmptyObject($("#myJingYing").val())) {
                        layer.msg("经营类目不能为空!"); return !1;
                    } else {
                        $.post("/User/myJingYing", {
                            v: $("#myJingYing").val()
                        }, function (data) {
                            layer.msg(data.Msg);
                        })
                    }
                });

                //更改工作年限
                $("#btn_myWorkYear").UnBindAndBind("click", function () {
                    if (t.tools.isEmptyObject($("#myWorkYear").val())) {
                        layer.msg("工作年限不能为空!"); return !1;
                    } else {
                        $.post("/User/myWorkYear", {
                            v: $("#myWorkYear").val()
                        }, function (data) {
                            layer.msg(data.Msg);
                        })
                    }
                })

                //日期选择控件
                lay('#setBirth').each(function () {
                    laydate.render({
                        elem: this,
                        trigger: 'click',
                        type: 'date',
                        min: "1950-01-01",
                        max: "{0}-12-31".Format(new Date().getFullYear() - 5),
                        calendar: !0,
                        showBottom: !1,
                        //btns: []
                    });
                });
            },
            mylike: function () {
                $(".likeaction").each(function (i, n) {
                    n.onmouseover = function () {
                        var $t = $(n);
                        var itme = $t.children().data("item");
                        if ($t.children().length == 1) {
                            $("<a>").data("item", itme).text("取消关注").addClass("btn-sm btn-primary").click(function (e) {
                                var item = $(this).data("item").split("|");
                                $.post("/bbs/like?t=" + t.getPK(), {
                                    id: item[0],
                                    type: item[1],
                                    unlike: true
                                }, function (data) {
                                    if (data.Ok) {
                                        layer.msg(data.Msg, function () {
                                            location.reload(!0);
                                        });
                                    } else {
                                        layer.msg(data.Msg || "失败");
                                    }
                                }).error(function (data) {
                                    alert(data.Msg || "提交异常，稍后再试!");
                                });
                            }).appendTo($t);
                        }
                    };
                    n.onmouseleave = function () {
                        $(n).find(".btn-sm.btn-primary").remove();
                    }
                });
                t.BootStrap_Tab_Change(".mylikeWrap", ".mylikeWrap", ".mylike_page_wrap", "/User/LoadMyPublish/7?{0}", "tab={0}".Format(location.pathname.split("/")[3]));
            },
            detail: function () {
                var funs = {
                    init: function () {
                        var me = this;
                        this.unlike(), this.like();
                        t.ChatInit();
                        $(".btn_like,.btn_unlike").unbind("click").click(function () {
                            var $this = $(this);
                            var f = $this.hasClass("btn_like") ? !1 : !0;
                            $.post("/bbs/like?t=" + t.getPK(), {
                                id: $this.data("id"),
                                type: 3,
                                unlike: f
                            }, function (data) {
                                if (data.Ok) {
                                    if (f) {
                                        $this.addClass("btn_like").removeClass("btn_unlike").text("关注"); me.like();
                                    } else {
                                        $this.addClass("btn_unlike").removeClass("btn_like").text("取消关注"); me.unlike();
                                    }
                                    $(".likecount").text(f ? (parseInt($(".likecount").text()) - 1) : (parseInt($(".likecount").text()) + 1))
                                    layer.msg(data.Msg);
                                } else {
                                    layer.msg(data.Msg || "失败");
                                }
                            }).error(function (data) {
                                alert(data.Msg || "提交异常，稍后再试!");
                            });
                        });
                        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                            var activeTab = $(e.target).data("reactid");
                            var previousTab = $(e.relatedTarget).text(),
                                tab = me.LoadDataType[activeTab - 1];
                            $(".tip_{0}".Format(tab)).removeAttr("href");
                            var tabParentElement = $(".tabs_{0}".Format(tab)).parent();
                            if (!tabParentElement.hasClass("active")) {
                                $("#userTabs .active").removeClass("active");
                                tabParentElement.addClass("active");
                            }
                            $("#{0}".Format(tab)).html('<div class="loadwrap"><img src="/Scripts/layer-1.8.5/skin/default/loading-0.gif" /></div>');
                            setTimeout(function () {
                                me.LoadEachData(tab);
                            }, t.tools.Random(500, 1000));
                        });
                        $(".likecount").unbind("click").bind("click", function () {
                            if (parseInt($(this).text()) > 0) {
                                me.LoadFans();
                            }
                        });
                        this.RunHash()
                    },
                    unlike: function () {
                        var ulike = $(".btn_unlike");
                        ulike.length > 0 && ulike.mouseover(function () {
                            $(this).text("取消关注")
                        }).mouseleave(function () {
                            $(this).text("已关注");
                        });
                        $(".btn_like").unbind("click");
                    },
                    like: function () {
                        var like = $(".btn_like");
                        like.length > 0 && like.on("mouseover mouseleave", function () {
                            $(this).text("关注")
                        });
                        $(".btn_unlike").unbind("click");
                    },
                    LoadEachData: function (datatype) {
                        var uri_ = "/User/{1}/{0}".Format(this.Gid(), datatype);
                        $.get(uri_, function (data) {
                            var renderElement = "#{0}".Format(datatype);
                            $(renderElement).html(data);
                            t.Page.PageRowNumber(renderElement, renderElement, uri_, "#userTabs", function () {
                                return t.LoadMask("#userTabContent", "-15px", "15px");
                            });
                        });
                    },
                    LoadDataType: ["question", "answer", "article", "bestAnswer", "niceAnswer"],
                    Gid: function () {
                        var duid, huid;
                        return (duid = $("#userTabs").data("uid"), huid = $("#hiduid").val(), duid == huid ? duid : huid);
                    },
                    LoadFans: function () {
                        var layerIndex = layer.load("正在加载粉丝数据");
                        $.get("/User/GetFans/{0}".Format(this.Gid()), function (data) {
                            layer.close(layerIndex);
                            if (data.Ok) {
                                if (data.Data.length > 0) {
                                    var list = data.Data;
                                    var html = [];
                                    $.each(list, function (i, n) {
                                        html.push("<div style='text-align:center;'><a href='/user/detail/{0}'>{0}</a></div>".Format(n["UserName"]));
                                    });
                                    var divEle = $("<div>");
                                    divEle.attr("id", "fansWrapper").addClass("Ldn").css("background-color", "#fff").append(html.join("")).appendTo($("body"));
                                    AlertDiv_End(divEle, "300px", "200px", "粉丝", "", function () {
                                        divEle.remove();
                                    });
                                }
                            } else {
                                layer.msg(data.Msg || "获取异常");
                            }
                        });
                    },
                    RunHash: function () {
                        var hash = location.hash
                        if (hash) {
                            var action = hash.replace("#", "");
                            if (action) {
                                action = action.toLowerCase();
                                switch (action) {
                                    case "question":
                                        $(".tabs_question").click();
                                        break;
                                    case "article":
                                        $(".tabs_article").click();
                                        break;
                                    case "fans":
                                        $(".likecount").click();
                                        break;
                                }
                            }
                        }
                    },
                }
                funs.init();
            },
            SE: function (arg) {
                var speed = arg[0];
                var _settimeout2;
                $("#btn_ScoreChange").unbind("click").bind("click", function () {
                    var _v = $("#ScoreChange");
                    if (t.tools.CheckFormNotEmpty(_v, "请输入要兑换VIP分个数")) {
                        $.post("/User/VipScore2Score/{0}".Format(_v.val()), function (data) {
                            if (data.Ok) {
                                layer.msg("兑换成功");
                                setTimeout(function () { location.reload(!0); }, 500);
                            } else {
                                layer.msg(data.Msg || "失败");
                            }
                        });
                    }
                });
                $("#ScoreChange").on("input change", function () {
                    if (_settimeout2) {
                        clearTimeout(_settimeout2);
                    }
                    var me = $(this);
                    _settimeout2 = setTimeout(function () {
                        var _mevalue = me.val();
                        if (_mevalue) {
                            var mevalue = parseInt(_mevalue),
                                memax = parseInt(me.attr("max"));
                            $(".ScorechangeResult").removeClass("Ldn").find("span").text(mevalue * speed);
                        } else {
                            $(".ScorechangeResult").addClass("Ldn");
                        }
                    }, 300);
                });
                $("#btn_VipScoreChange").unbind("click").bind("click", function () {
                    var _v = $("#VipScoreChange");
                    if (t.tools.CheckFormNotEmpty(_v, "请输入要兑换积分个数")) {
                        $.post("/User/Score2VipScore/{0}".Format(_v.val()), function (data) {
                            if (data.Ok) {
                                layer.msg("兑换成功");
                                setTimeout(function () { location.reload(!0); }, 500);
                            } else {
                                layer.msg(data.Msg || "失败");
                            }
                        });
                    }
                });
                var _settimeout;
                $("#VipScoreChange").on("input change", function () {
                    if (_settimeout) {
                        clearTimeout(_settimeout);
                    }
                    var me = $(this);
                    _settimeout = setTimeout(function () {
                        var _mevalue = me.val();
                        if (_mevalue) {
                            var mevalue = parseInt(_mevalue),
                                memax = parseInt(me.attr("max")),
                                _value = 0;
                            if (mevalue < speed) {
                                _value = speed;
                                me.val(_value);
                            } else if (mevalue > memax) {
                                _value = memax;
                                me.val(_value);
                            }
                            else {
                                _value = mevalue;
                            }
                            _value = Math.floor(_value / speed) * speed;
                            me.val(_value);
                            $(".VipchangeResult").removeClass("Ldn").find("span").text(_value / speed);
                        } else {
                            $(".VipchangeResult").addClass("Ldn");
                        }
                    }, 300);
                });
                var money = arg[1];
                //购买会员
                $(".btn-buyVIP").click(function () {
                    var me = $(this);
                    var month = parseInt(me.data("month"));
                    var layerIndex = AlertConfirm("你确定花费{0}VIP分来购买{1}个月的VIP会员吗？".Format(money * month, month), "确定购买", "暂不购买", function () {
                        layer.close(layerIndex);
                        layerIndex = layer.load("正在购买中……");
                        $.post("/User/BuyVIP/{0}".Format(month), function (data) {
                            layer.close(layerIndex);
                            if (data.Ok) {
                                layer.msg("会员购买成功", {
                                    time: 300, end: function () { location.href = "/user"; }
                                });
                            } else {
                                if (data.ID == 1) {
                                    layerIndex = AlertConfirm(data.Msg, "前往", "暂不", function () {
                                        layer.close(layerIndex);
                                        location.href = data.Url;
                                    });
                                }
                                else {
                                    layer.msg(data.Msg);
                                }
                            }
                        });
                    });
                });
            },
            ReCharge: function (speed) {
                $("#btn_VipScoreReCharge").unbind("click").bind("click", function () {
                    var _val = $("#vipscorerecharge");
                    if (t.tools.CheckFormNotEmpty(_val, "请输入充值金额(RMB)")) {
                        $.post("/Pay/Pay?t=" + t.getPK(), { fee: _val.val(), desc: "VIP分充值", id: 0, type: 1 }, function (data) {
                            if (data.Ok) {
                                location.href = "/pay/payredirect";
                            } else {
                                layer.msg(data.Msg || "充值失败");
                            }
                        });
                    }
                });
            },
            userauth: function () {
                var check = t.tools.CheckFormNotEmpty;
                $('.btn-subAuth').unbind("click").bind("click", function () {
                    var name = "#TrueName", cardId = "#MyCardID", CardPic = "#CardPic", selectCardPic = "#selectCardPic", FaRenPic = "#FaRenPic", selectFaRenPic = "#selectFaRenPic", companyName = "#companyName", companyTel = "#companyTel";
                    if (check(name, "姓名")) {
                        if (check(cardId, "身份证号码")) {
                            if (check(CardPic, "身份证照片")) {
                                if (check(FaRenPic, "营业执照照片")) {
                                    if (check(companyName, "公司单位名称")) {
                                        if (check(companyTel, "公司联系电话")) {
                                            var formdata = new FormData();
                                            formdata.append("CardPic", $(selectCardPic)[0].files[0]);
                                            formdata.append("FaRenPic", $(selectFaRenPic)[0].files[0]);
                                            formdata.append("truename", name.val());
                                            formdata.append("card", cardId.val());
                                            formdata.append("companyname", companyName.val());
                                            formdata.append("companyTel", companyTel.val());
                                            $.ajax({
                                                url: "/User/Auth",
                                                type: "post",
                                                dataType: "json",
                                                contentType: false,
                                                processData: false,
                                                data: formdata,
                                                success: function (data) {
                                                    if (data.Ok) {
                                                        name.val("");
                                                        cardId.val("");
                                                        CardPic.val("");
                                                        selectCardPic.val("");
                                                        FaRenPic.val("");
                                                        selectFaRenPic.val("");
                                                        companyName.val("");
                                                        companyTel.val("");
                                                        MSG("提交法人认证申请成功，等待管理员审核", function () {
                                                            location.href = "/user";
                                                        });
                                                    } else {
                                                        layer.msg(data.Msg || "提交失败");
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
                t.tools.PickupInput(selectCardPic, CardPic);
                t.tools.PickupInput(selectFaRenPic, FaRenPic);
            },
            myshare: function () {
                //领取奖励
                $(".btn-getShareCoin").unbind("click").bind("click", function () {
                    var me = $(this);
                    var layerIndex = layer.load("领取奖励中");
                    $.post("/Share/GetShareCoin", { coin: me.data("coin"), coinID: me.data("coinid") }, function (data) {
                        if (data.Ok) {
                            me.unbind("click");
                            me.addClass("btn-default disabled").removeClass("btn-getShareCoin btn-primary");
                            me.text("已领取");
                            me.data("coin", undefined);
                            me.data("coinid", undefined);
                            layer.msg("奖励领取成功");
                        } else {
                            layer.msg("领取异常");
                        }
                        layer.close(layerIndex);
                    });
                });
                t.InitClipboard(".btn-paste");
            },
            inbox: function () {
                t.ChatInit();
                //function onClick() {
                //    $(".btn_dialog").UnBindAndBind("click", function () {
                //        var me = $(this);
                //        window.open(location.origin + "/user/dialog/" + me.data("id"));
                //    });
                //}
                t.Page.PageAppend(".inbox-list", ".aw-feed-list", "/user/inbox", null, function () {
                    //onClick();
                    t.ChatInit();
                });
                //onClick();
            },
            dialog: function (toid) {
                t.Page.PageAppend(".dialogList", ".aw-feed-list", "/user/dialog/{0}".Format(toid));
                t.ChatInit();
            },
            notice: function () {
                t.Page.PageAppend("#Noticepage", ".noticeList", "/User/Notice", !0);
                var notices = $(".noticeIcon");
                if (notices.length > 0) {
                    setInterval(function () {
                        if (!notices.data("icon")) {
                            notices.css("transform", "scale(1.05)").data("icon", !0);
                        } else {
                            notices.css("transform", "scale(1)").data("icon", !1);
                        }
                    }, 1000);
                    $.post("/Notice/Read");
                }
            },
            myanswer: function () {
                t.BootStrap_Tab_Change(".mycenterRight", ".mycenterRight", ".myanswer_page_wrap", "/User/LoadMyPublish/2");
            },
            myarticle: function () {
                t.BootStrap_Tab_Change(".mycenterRight", ".mycenterRight", ".myarticle_page_wrap", "/User/LoadMyPublish/6");
            },
            myquestion: function () {
                t.BootStrap_Tab_Change(".mycenterRight", ".mycenterRight", ".myquestion_page_wrap", "/User/LoadMyPublish/1");
            },
            myzhaopin: function () {
                $(".btn-light").UnBindAndBind("click", function () {
                    var me = $(this);
                    var lindex = LOAD("正在擦亮招聘信息中...");
                    $.post("/zhaopin/Light/{0}".Format(me.data("mid")), function (data) {
                        if (data.Ok) {
                            t.msgsuccess(data.Msg);
                            me.unbind("click");
                            me.removeClass("btn-success").addClass("btn-warning disabled").text("无需擦亮");
                            me.parent().parent().find(".validShow").empty().append($("<span>").text(t.tools.ChangeDateFormat(data.Data)));
                        } else {
                            t.msgfail(data.Msg || "擦亮失败");
                        }
                        CLOSE(lindex);
                    });
                });
                t.BootStrap_Tab_Change(".mycenterRight", ".mycenterRight", ".myzhaopinpage", "/User/LoadMyPublish/3");
            },
            myqiuzhi: function () {
                $(".btn-light").UnBindAndBind("click", function () {
                    var me = $(this);
                    var lindex = LOAD("正在擦亮求职信息中...");
                    $.post("/qiuzhi/light/{0}".Format(me.data("mid")), function (data) {
                        if (data.Ok) {
                            t.msgsuccess(data.Msg);
                            me.unbind("click");
                            me.removeClass("btn-success").addClass("btn-warning disabled").text("无需擦亮");
                            me.parent().parent().find(".validShow").empty().append($("<span>").text(t.tools.ChangeDateFormat(data.Data)));
                        } else {
                            t.msgfail(data.Msg || "擦亮失败");
                        }
                        CLOSE(lindex);
                    });
                });
                t.BootStrap_Tab_Change(".mycenterRight", ".mycenterRight", ".myqiuzhipage", "/User/LoadMyPublish/4");
            },
            myproduct: function () {
                $(".btn-light").UnBindAndBind("click", function () {
                    var me = $(this);
                    var lindex = LOAD("正在擦亮产品信息中...");
                    $.post("/product/light/{0}".Format(me.data("mid")), function (data) {
                        if (data.Ok) {
                            t.msgsuccess(data.Msg);
                            me.unbind("click");
                            me.removeClass("btn-success").addClass("btn-warning disabled").text("无需擦亮");
                            me.parent().parent().find(".validShow").empty().append($("<span>").text(t.tools.ChangeDateFormat(data.Data)));
                        } else {
                            t.msgfail(data.Msg || "擦亮失败");
                        }
                        CLOSE(lindex);
                    });
                });
                t.BootStrap_Tab_Change(".mycenterRight", ".mycenterRight", ".myproductpage", "/User/LoadMyPublish/5");
            },
            order: function () {
                function EachList(data, element, callback) {
                    if (data.Ok) {
                        data = data.Data;
                        var divWrap;
                        if (data.length > 0) {
                            divWrap = $("<table>").addClass("batchTags_Wrap table table-bordered table-hover Ltac");
                            divWrap.append($("<tr>").append(
                                $("<th>").addClass("Ltac").text("商品"),
                                $("<th>").addClass("Ltac").text("消费金额"),
                                $("<th>").addClass("Ltac").text("消费积分"),
                                $("<th>").addClass("Ltac").text("消费VIP分"),
                                $("<th>").addClass("Ltac").text("下单时间"),
                                $("<th>").addClass("Ltac").text("发货预计时间"),
                                $("<th>").addClass("Ltac").text("发货结果"),
                                $("<th>").addClass("Ltac").text("收货结果"),
                                $("<th>").addClass("Ltac").text("付款否"),
                                $("<th>").addClass("Ltac").text("手机"),
                                $("<th>").addClass("Ltac").text("联系人"),
                                $("<th>").addClass("Ltac").text("联系小二"),
                                $("<th>").addClass("Ltac").text("其他")
                            ))
                            $.each(data, function (i, n) {
                                var cb = callback(n);
                                cb && cb.appendTo(divWrap);
                            });
                        } else {
                            divWrap = NoHtmlList();
                        }
                        $(element).html(divWrap);
                    }
                }

                function BuildListHtml(n, type, url) {
                    if (n.ginfo) {
                        var tr = $("<tr>");
                        tr.append($("<td>").text(n.ginfo.name));
                        //0免费  1 积分 2VIP分 3现金
                        tr.append($("<td>").text(n.payType == 3 ? n.fee : "-"));
                        tr.append($("<td>").text(n.payType == 1 ? n.fee : "-"));
                        tr.append($("<td>").text(n.payType == 2 ? n.fee : "-"));
                        tr.append($("<td>").text(t.tools.ChangeDateFormat(n.buyTime)));
                        //如果买家付款了
                        if (n.ispay) {
                            if (n.sendInfo) {
                                if (n.sendInfo.status == 0) {
                                    tr.append($("<td>").text(t.tools.ChangeDateFormat(n.sendTime)));//发货预计时间
                                    tr.append($("<td>").text("待发货"));
                                } else {
                                    tr.append($("<td>").text("-"));
                                    tr.append($("<td>").text("已发货"));
                                }
                            } else {
                                tr.append($("<td>").text("-"));
                                tr.append($("<td>").text("-"));
                            }
                            if (n.checkInfo) {
                                if (n.checkInfo.status == 0) {
                                    tr.append($("<td>").append($("<a>").addClass("btn btn-sm btn-primary").text("确认收货").click(function () {
                                        AlertConfirm("请确认收到商品再进行确认收货", "确证收货", "取消", function () {
                                            $.post("/user/confirmreceipt/{0}?gid={1}&mid={2}".Format(n.checkInfo.cid, n.ugid, n.ginfo.gid), function (res) {
                                                if (res.Ok) {
                                                    t.msgsuccess(res.Msg || "收货成功", function () {
                                                        location.reload();
                                                    }, 500);
                                                } else {
                                                    t.msgfail("收货失败", function () {
                                                        location.reload();
                                                    }, 500);
                                                }
                                            });
                                        });
                                    })));
                                }
                                else if (n.checkInfo.status == 1) {
                                    tr.append($("<td>").text("已收货"));
                                }
                                else if (n.checkInfo.status == 2) {
                                    tr.append($("<td>").text("退款中"));
                                }
                                else if (n.checkInfo.status == 3) {
                                    tr.append($("<td>").text("已退款"));
                                }
                            } else {
                                tr.append($("<td>").text("已收货"));
                            }
                        } else {
                            tr.append($("<td>").text("-"));
                            tr.append($("<td>").text("-"));
                            tr.append($("<td>").text("-"));
                        }
                        tr.append($("<td>").text(n.ispay ? "已付款" : "未付款"));
                        tr.append($("<td>").text(n.tel || "-"));
                        tr.append($("<td>").text(n.receiveName || "-"));
                        tr.append($("<td>").append($("<a>").addClass("btn btn-sm btn-paimary {0}".Format(t.user.id == 10006 ? "kwkw" : "btn_sendMsg")).text("联系小二").data({ "id": "10006", "name": "客服小二" }).click(function () {
                            if (t.user.id == 10006) {
                                t.msgfail("不能自己联系自己");
                            }
                        })));
                        tr.append($("<td>").text("-"));
                        return tr;
                    }
                }

                function LoadALLOrder() {
                    $.get("/user/loadallorder", function (data) {
                        EachList(data, "#load-allorder", function (n) {
                            return BuildListHtml(n, 2, t.baseUrl + "/gift/detail/{0}");
                        });
                        t.ChatInit();
                    });
                }

                function LoadToCheck() {
                    $.get("/user/loadtocheckorder", function (data) {
                        EachList(data, "#load-tocheck", function (n) {
                            return BuildListHtml(n, 4, Leo.baseUrl + "/gift/detail/{0}");
                        });
                        t.ChatInit();
                    });
                }

                function LoadFinished() {
                    $.get("/user/loadfinishedorder", function (data) {
                        EachList(data, "#load-finished", function (n) {
                            return BuildListHtml(n, 8, Leo.baseUrl + "/gift/detail/{0}");
                        });
                        t.ChatInit();
                    });
                }

                function LoadWrap(element) {
                    $(element).html('<div class="loadwrap">正在整理数据，请稍等<img src="/Scripts/layer-1.8.5/skin/default/loading-0.gif" /></div>');
                }

                function NoHtmlList() {
                    return $('<div style="text-align:center;"><div class="well"><h4>无订单</h4></div></div>');
                }

                $(function () {
                    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                        var index = $(e.target).data("index");
                        $(".setBtn").addClass("Ldn");
                        if (index == "1") {
                            LoadWrap("#load-gift");
                            LoadALLOrder();
                        } else if (index == "2") {
                            LoadWrap("#load-tocheck");
                            LoadToCheck();
                        } else {
                            LoadWrap("#load-finished");
                            LoadFinished();
                        }
                    });
                });

                LoadALLOrder();
            }
        };
        return function (action, p) {
            return T[action](p);
        }
    },
    party: function () {
        var t = this;
        var T = {
            ReSizeIndexPic: function () {
                var items = $(".partyItem");
                if (items.length > 0) {
                    var gwidth = items.eq(0).width();
                    $.each(items, function () {
                        $(this).find("a div").eq(0).height(gwidth / 1.5);
                    });
                }
            },
            "index": function () {
                var me = T;
                t.Search(function (keyword) {
                    var layerIndex = LOAD("正在搜索...");
                    $.get("/party/Search", { key: keyword }, function (data) {
                        if (data.Ok === false) {
                            layer.msg(data.Msg);
                        } else {
                            $(".party_List").html(data);
                            me.ReSizeIndexPic();
                        }
                        CLOSE(layerIndex);
                    });
                });
                me.ReSizeIndexPic();
                t.BootStrap_Tab_Change(".party_List", ".party_List", ".party_Page_Wrap", "/party/LoadMore", null, null, null, function () {
                    me.ReSizeIndexPic();
                });
            },
            "detail": function (p) {
                $(".btn_IJoin").click(function () {
                    var me = $(this), _feeselected = $(".detail_join_ticket_con .ticket li.select table .ticket_money");
                    if (_feeselected.length > 0) {
                        var _fee = parseFloat(_feeselected.data("fee") || 0), _feeIden = _feeselected.data("feeidn"), _feetype = _feeselected.data("feetype"), _msg = _fee == 0 ? "报名免费" : "{0}".Format(_fee), buycount = $(".select_num .count input").val(), hasBuy = me.hasClass("hasBuy"), buymsgtip = "温馨提示：";
                        if (hasBuy) {
                            buymsgtip = "您已经报名";
                            var buyitems = me.data("joinitem");
                            $.each(buyitems, function (i, n) {
                                buymsgtip += "{0}份【{1}】票".Format(buyitems[i].count, buyitems[i].name);
                                if (i < buyitems.length - 1) {
                                    buymsgtip += ",";
                                }
                            });
                        }
                        //////////////////////
                        var _desc = _feetype == 0 ? "{0}" : _feetype == 10 ? "积分付费{0}" : _feetype == 20 ? "VIP分付费{0}" : "人民币付费{0}元";
                        var layerIndex = AlertConfirm("{2}<br/>您本次报名信息如下：<br>数量：{0}份<br>付费类型：{1}<br>你确定要报名吗？".Format(buycount, _desc.Format(_fee == 0 ? _msg : "<br>总费用：{0}".Format(_fee * buycount)), buymsgtip), "填资料", "我再想想",
                            function () {
                                CLOSE(layerIndex);
                                var h = $(".btn-subInfoAndJoin").data("height");
                                layerIndex = AlertDiv_End(".baseInfoToJoin", $(window).width() > 418 ? "418px" : "370px", h || "200px", "填写报名基本资料", null, function () { });
                                $(".btn-subInfoAndJoin").UnBindAndBind("click", function () {
                                    var wok = !1, par = { JoinItems: [] }, joinitems = $(".joinInputForm"), joinItemcount = joinitems.length;
                                    for (var i = 1; i <= joinItemcount; i++) {
                                        var item = $(".joinInputForm{0}".Format(i));
                                        if (t.tools.CheckFormNotEmpty(item, item.attr("placeholder"))) {
                                            if (t.tools.CheckLength(item.val(), 1, 100, item.attr("placeholder"))) {
                                                wok = !0;
                                                i < 3 ? par["{0}".Format(i == 1 ? "LinkMan" : "LinkTel")] = item.val() : par.JoinItems.push({ Id: item.data("id"), Value: item.val() })
                                            } else {
                                                par = {};
                                                return;
                                            }
                                        } else {
                                            par = {}; return;
                                        }
                                    }
                                    par.fee = _fee;
                                    par.feeid = _feeIden;
                                    par.count = buycount;
                                    wok && (_feetype != 30 ? (function () {
                                        var layerMask = LOAD("正在报名...");
                                        $.post("/party/submit/{1}?t={0}".Format(Leo.getPK(), me.data("aid")), par, function (data) {
                                            if (data.Ok) {
                                                layer.closeAll();
                                                $(".baseInfoToJoin").find(".joinInputForm").val("");
                                                t.MsgTips("恭喜您报名成功", data.Msg || "报名成功");
                                                var selectTickets = $(".tickets li.select"), numEle = selectTickets.find(".num"), nowCount = parseInt(numEle.data("count")) - par.count;
                                                if (nowCount <= 0) {
                                                    selectTickets.removeClass("select").addClass("gray end");
                                                    numEle.data("count", 0).text("剩余0张");
                                                } else {
                                                    numEle.data("count", nowCount).text("剩余{0}张".Format(nowCount));
                                                    $(".select_num .item_limit_note").text("剩余{0}张".Format(nowCount));
                                                }
                                                var noticeBox = $(".bxtnoticebox");
                                                noticeBox.removeClass("Ldni").find(".noticecount").text(1);
                                            } else {
                                                t.msgfail(data.Msg, function () {
                                                    if (data.Type == 1) {
                                                        location.reload(!0);
                                                    }
                                                });
                                            }
                                        });
                                    })() : (function () {
                                        var layerOrder = LOAD("正在生成订单...");
                                        par.type = 3;
                                        par.id = _feeIden;
                                        par.mid = me.data("aid");
                                        par.desc = "百晓堂参加活动" + par.mid;
                                        setTimeout(function () {
                                            $.post("/Pay/Pay", par, function (data) {
                                                if (data.Ok) {
                                                    location.href = data.Url + "?t=" + t.getPK();
                                                } else {
                                                    //layer.close(layerOrder);
                                                    layer.closeAll();
                                                    t.msgfail(data.Msg, function () {
                                                        if (data.Type == 1) {
                                                            location.reload(!0);
                                                        }
                                                    });
                                                }
                                            });
                                        }, t.tools.Random(500, 1000));
                                    })());
                                });
                            }
                        );
                    } else {
                        t.msgfail("请先选择任一票种类型进行购买！");
                    }
                    //////////////////////
                });
                //选择票种
                $(".tickets li").click(function () {
                    var _me = $(this);
                    if (!_me.hasClass("gray end")) {
                        $(".tickets li.select").removeClass("select");
                        _me.addClass("select");
                        $(".item_limit_note").html(_me.find("table").find(".num").text());
                        $('.select_num input').val(1);
                    }
                }).mouseover(function () {
                    $(this).addClass("select_hover");
                }).mouseleave(function () {
                    $(this).removeClass("select_hover");
                });
                $.get("/party/GetJoinedUser/{0}".Format(p), function (data) {
                    if (data.Data != null && data.Data.length > 0) {
                        $(".titleBuyCount").text(data.Data.length);
                        var defaultUrl = "/Content/img/head_default.gif";
                        var wrap = $(".joinedUserList ul");
                        $.each(data.Data, function (i, n) {
                            wrap.append($("<li>").addClass("Lfl").append($("<a>").css({ display: "block" }).attr({ "data-toggle": "tooltip", title: n["UserName"], target: "_blank", "href": "/User/detail/{0}".Format(n["UserName"]) }).append($("<img>").attr({ "src": n["HeadUrl"] || defaultUrl, width: 48 }))));
                        });
                        t.ToolTip();
                    }
                });
                //加减数量
                var verifyCount = function (onfocus) {
                    var selectCount = select_num.find("input"), countVal = parseInt(selectCount.val()) || (onfocus ? "" : 1),
                        totalCount = parseInt($(".tickets li.select .num").data("count") || 0);
                    if ((onfocus && countVal) || !onfocus) {
                        if (countVal >= totalCount) {
                            countVal = totalCount;
                            t.msgfail("余票只剩{0}张！".Format(totalCount));
                        } else {
                            onfocus || countVal++;
                        }
                        selectCount.val(countVal);
                    }
                }
                var select_num = $('.select_num');
                select_num.find("input").blur(function () {
                    var me = $(this), _v = me.val();
                    (!_v || !parseInt(_v)) && me.val(1);
                }).on("paste keyup", function () {
                    verifyCount(!0);
                });
                select_num.find(".less").UnBindAndBind("click", function () {
                    var selectCount = select_num.find("input"), countVal = parseInt(selectCount.val()) || 1;
                    if (countVal > 1) {
                        selectCount.val(countVal - 1);
                    } else {
                        t.msgfail("已经是1张了，不能再低了！");
                        selectCount.val(1);
                    }
                });
                select_num.find(".add").UnBindAndBind("click", function () {
                    verifyCount(!1);
                });
            },
            publish: function () {
                var maxItem = 100, moneyCount = 100, deleteFeeBox = [], deleteJoinBox = [];
                var regu = new RegExp("^([0-9]*[.0-9])$"); //小数测试
                var reg = new RegExp("^[0-9]*[1-9][0-9]*$");
                //return
                $("#btn_addParty").UnBindAndBind("click", function () {
                    if (checkEmpty("#Party_Title", "活动主题")) { return !1; }
                    if (!$(".showactiveimg").data("isTemplate")) {
                        if (checkEmpty("#Party_ActivityIMG", "活动海报")) { $(window).scrollTop(0); $("#btn_addActiveImg").click(); return !1; }
                    }
                    //if (checkEmpty("#Party_BeginTime", "活动开始时间")) { return !1; }
                    //if (checkEmpty("#Party_EndTime", "活动结束时间")) { return !1; }
                    if ((checkEmpty("#SelectBeginTime", "活动开始时间"))) { return !1; }
                    if (!$("#Party_CanJoinOnBeginCHeck").is(":checked")) {
                        //if (checkEmpty("#Party_JoinBeginTime", "报名开始时间")) { return !1; }
                        //if ((checkEmpty("#Party_JoinEndTime", "报名截止时间"))) { return !1; }
                        if (checkEmpty("#SelectJoinTime", "活动报名时间")) { return !1; }
                    }
                    if (checkEmpty("#Party_Address", "活动地址")) { return !1; }
                    if ($('#summernote').summernote("isEmpty")) { layer.msg("活动详情不能为空"); return !1; }
                    else {
                        var feeok = true, saveIndexItem = [], maxIndex = 0;
                        for (var i = 0; i < moneyCount; i++) {
                            if ($(".feeitemwrap-{0}".Format(i)).length > 0) {
                                if (checkEmpty("#pickupFee{0}".Format(i), "费用类型")) { feeok = false; return !1; }
                                if (checkEmpty("#PartyFee_{0}__FeeName".Format(i), "票种名称")) { feeok = false; return !1; }
                                if (t.tools.CheckLength($("#PartyFee_{0}__FeeName".Format(i)).val(), 1, 15, "票种名称")) {
                                    if ($("#pickupFee").val() !== "{0}") {
                                        if (checkEmpty("#PartyFee_{0}__Fee".Format(i), "报名费")) { feeok = false; return !1; }
                                    }
                                    if (checkEmpty("#PartyFee_{0}__FeeCount".Format(i), "活动名额")) { feeok = false; return !1; }
                                    if ($("#pickupFee{0}".Format(i)).val() != 0 && $("#PartyFee_{0}__Fee".Format(i)).val() == 0) { $("#pickupFee{0}".Format(i)).focus(); Leo.msgfail("费用为0的话，请选择免费"); feeok = false; return !1; }
                                } else {
                                    $("#PartyFee_{0}__FeeName".Format(i)).focus();
                                    feeok = false;
                                    return !1;
                                }
                                maxIndex = i;
                            } else {
                                saveIndexItem.push(i);
                            }
                        }
                        if (feeok) {
                            $.each(saveIndexItem, function (i, n) {
                                if (n < maxIndex) {
                                    var html = [];
                                    html.push('<div class="feeitemwrap feeitemwrap-{0} col-md-10 Lpdl0 Ldn">'.Format(n));
                                    html.push('<input data-val="true" data-val-number="The field FeeType must be a number." id="PartyFee_{0}__FeeType" name="PartyFee[{0}].FeeType" type="hidden" value="">'.Format(n));
                                    html.push('<input class="form-control" id="PartyFee_{0}__FeeName" name="PartyFee[{0}].FeeName" placeholder="票种名称" type="text" value="">'.Format(n));
                                    html.push('<input class="form-control" data-val="true" data-val-number="The field Fee must be a number." id="PartyFee_{0}__Fee" min="0" name="PartyFee[{0}].Fee" placeholder="报名费" type="number" value="">'.Format(n));
                                    html.push('<input class="form-control" data-val="true" data-val-number="The field FeeCount must be a number." id="PartyFee_{0}__FeeCount" min="1" name="PartyFee[{0}].FeeCount" placeholder="活动名额" type="number" value="">'.Format(n));
                                    html.push("</div>");
                                    $(".feetypeList").append(html.join(''));
                                }
                            });
                            var joinSaveIndexItem = [], maxJoinIndex = 0;
                            for (var i = 0; i < maxItem; i++) {
                                var joinItem = "#JoinItemQues_{0}__ItemName".Format(i);
                                if ($(joinItem).length > 0) {
                                    if (checkEmpty(joinItem, "报名项名称")) {
                                        feeok = false; return !1;
                                    } else {
                                        feeok = t.tools.CheckLength(joinItem.val(), 1, 10, "报名项名称");
                                    }
                                    if (feeok) {
                                        maxJoinIndex = i;
                                    }
                                }
                                else {
                                    joinSaveIndexItem.push(i);
                                }
                            }
                            if (feeok) {
                                $.each(joinSaveIndexItem, function (i, n) {
                                    if (n < maxJoinIndex) {
                                        var html = [];
                                        html.push('<div class="joinItemWrap joinItemWrap-{0} col-md-offset-2 col-md-10 Lpdl0 Lmgt5 Ldn">'.Format(n));
                                        html.push('<input id="JoinItemQues_{0}__ItemName" name="JoinItemQues[{0}].ItemName" class="form-control" type="hidden" placeholder="报名项名称">'.Format(n));
                                        html.push("</div>");
                                        $(".joinBaseInfos").append(html.join(''));
                                    }
                                });
                                var laydrIndex = LOAD("正在发布活动...");
                                $("#Party_Body").val(encodeURI($('#summernote').summernote("code")));
                                if ($("#Party_CanJoinOnBeginCHeck").is(":checked")) {
                                    $("#Party_CanJoinOnBegin").val(1);
                                } else {
                                    $("#Party_CanJoinOnBegin").val(2);
                                    //包装报名时间
                                    var jointimes = $("#SelectJoinTime").val().split(" - ");
                                    $("#Party_JoinBeginTime").val(jointimes[0]);
                                    $("#Party_JoinEndTime").val(jointimes[1]);
                                }

                                //包装活动时间
                                var jointimes = $("#SelectBeginTime").val().split(" - ");
                                $("#Party_BeginTime").val(jointimes[0]);
                                $("#Party_EndTime").val(jointimes[1]);

                                //检查是否为自定义上传海报
                                if ($(".showactiveimg").data("isTemplate")) {
                                    //上传
                                    var formdata = new FormData();
                                    formdata.append("file", $("#addDIYActiveImg_VALUE")[0].files[0]);
                                    Leo.SendFile("/tool/ActiveFinalImg", $("#addDIYActiveImg_VALUE")[0], !1, function (data) {
                                        $("#Party_ActivityIMG").val(data.Url);
                                        $("#nmform").ajaxSubmit(function (data) {
                                            if (data.Ok) {
                                                layer.msg("新活动创建成功");
                                                location.href = data.Url;
                                            } else {
                                                layer.msg(data.Msg || "创建失败");
                                            }
                                            CLOSE(laydrIndex);
                                        });
                                    });
                                } else {
                                    $("#nmform").ajaxSubmit(function (data) {
                                        if (data.Ok) {
                                            layer.msg("新活动创建成功");
                                            location.href = data.Url;
                                        } else {
                                            layer.msg(data.Msg || "创建失败");
                                        }
                                        CLOSE(laydrIndex);
                                    });
                                }
                            }
                        }
                    }
                });

                $("#Party_CanJoinOnBeginCHeck").change(function () {
                    if (this.checked) {
                        $(".setJoinTime").addClass("Ldn").children().val("");
                    } else {
                        $(".setJoinTime").removeClass("Ldn").children().val("");
                    }
                });

                $("#btn_addDIYActiveImg").click(function () {
                    $("#addDIYActiveImg_VALUE").click();
                });

                var lindex;
                $("#btn_addActiveImg").click(function () {
                    if ($("#ActivityIMGS .leo").children().length == 1) {
                        var imgs = ["juhui", "lvyou", "shangwu", "qinzi", "wenhua", "yundong", "yanchu"];
                        //var html = [];
                        $.each(imgs, function (i, n) {
                            for (var i = 1; i < 13; i++) {
                                var divEle = $("<div>").addClass("imgwrap col-sm-6 col-md-4");
                                var aEle = $("<a>").addClass("thumbnail");
                                var imgEle = $("<img>").attr({ "title": "点击选择", "src": "/Content/img/party/poster_image_{0}_{1}.jpg".Format(n, i) }).click(function () {
                                    var src = $(this).attr("src");
                                    $("#Party_ActivityIMG").val(src);
                                    $("#btn_addActiveImg").prev().attr("src", src);
                                    layer.close(lindex);
                                });
                                $("#ActivityIMGS .leo").append(divEle.append(aEle.append(imgEle)));
                            }
                        });
                    }

                    lindex = layer.open({
                        scrollbar: !1,
                        type: 1,
                        title: false,
                        closeBtn: 0,
                        area: ["620px", "410px"],
                        shadeClose: !0,//单击遮罩层关闭窗口
                        content: $("#ActivityIMGS")
                    });
                });

                $("#addDIYActiveImg_VALUE").change(function () {
                    var target = this;
                    //上传图片到临时文件夹
                    Leo.SendFile("/tool/ActiveTemplateImg", target, !1, function (data) {
                        $(".showactiveimg").attr("src", data.Url);
                        $(".showactiveimg").data("isTemplate", !0);
                    });
                });

                function PICKUPIMG(target) {
                    var src = $(target).attr("src");
                    $("#Party_ActivityIMG").val(src);
                    $("#btn_addActiveImg").prev().attr("src", src);
                    layer.close(lindex);
                }

                function checkEmpty($input, msg) {
                    if ($($input).length) {
                        if (Leo.tools.isEmptyObject($.trim($($input).val()))) {
                            $($input).focus();
                            layer.msg((msg || "") + "不能为空");
                            return true;
                        }
                        else {
                            return false;
                        }
                    } { return false; }
                }

                function InitLayDateCSS(wrapElement) {
                    var browser = t.browser;
                    if (browser.versions.ios || browser.versions.android || browser.versions.iPhone) {
                        var laydate = $(wrapElement);
                        laydate.css({ "font-size": "12px", "width": "100%", "left": "0" });
                        laydate.find(".layui-laydate-main").css("width", "50%");
                        var laydatelist0 = laydate.find(".laydate-main-list-0");
                        var laydatelist1 = laydate.find(".laydate-main-list-1");
                        laydatelist0.find(".layui-laydate-header").css("padding-right", 0).find(".laydate-set-ym").css("text-align", "left");
                        laydatelist0.find(".layui-laydate-content").css("padding", 0);
                        laydatelist1.find(".layui-laydate-header").css("padding-left", 0).find(".laydate-set-ym").css("text-align", "right");
                        laydatelist1.find(".layui-laydate-content").css("padding", 0);
                    }
                }

                $.each(["#SelectBeginTime", "#SelectJoinTime"], function (i, n) {
                    laydate.render({
                        elem: n
                        , type: 'datetime'
                        , range: true
                        , trigger: 'click'
                        , min: new Date().getFullYear().toString() + "-" + (new Date().getMonth() + 1).toString() + "-" + new Date().getDate().toString()
                        , ready: function (index) {
                            InitLayDateCSS("#layui-laydate1");
                        }
                        , done: function (value, date) {
                            if (n == "#SelectJoinTime") {
                                var _starttime = $("#SelectBeginTime");
                                if (t.tools.CheckFormNotEmpty(_starttime, "请先选择活动时间！", !1)) {
                                    if (t.tools.DateCompare(_starttime.val().split(" - ")[0], value.split(" - ")[1]) == 1) {
                                        t.msgfail("活动报名截止时间必须在活动开始时间之前！");
                                        $("#SelectJoinTime").focus();
                                        setTimeout(function () { $("#SelectJoinTime").val(""); }, 1);
                                    }
                                } else {
                                    setTimeout(function () { $("#SelectJoinTime").val(""); }, 1);
                                }
                            }
                        }
                    });
                });

                Leo.RichText("#summernote", "/tool/uploadimg", "请输入活动内容", 300, !1, function (data) {
                    $('#summernote').summernote('insertImage', data.Url);
                });

                function InitFeeTypeChange(index) {
                    $("#pickupFee{0}".Format(index)).change(function () {
                        $(this).children().length == 5 && $(this).children().eq(0).remove();
                        $("#PartyFee_{0}__FeeType".Format(index)).val(this.value);
                        if (this.value == "0") {
                            $("#PartyFee_{0}__Fee".Format(index)).attr("readonly", !0).val("0");
                            $("#PartyFee_{0}__FeeName".Format(index)).val("福利票");
                        } else {
                            $("#PartyFee_{0}__Fee".Format(index)).removeAttr("readonly").val("");
                            $("#PartyFee_{0}__FeeName".Format(index)).val("");
                        }
                    });
                    $("#PartyFee_{0}__Fee".Format(index)).on("blur change paste", function () {
                        var item = $("#PartyFee_{0}__FeeType".Format(index)), me = $(this);
                        if (item.val() !== "30") {
                            var v = me.val();
                            if (!Leo.tools.isEmptyObject(v)) {
                                if (v.search(regu) == -1) {
                                    Leo.msgfail("非现金支付时不能为小数，只能为整数！");
                                    me.css("border-color", "red").focus();
                                }
                            }
                        }
                    });
                }

                function addNewFeeType(index) {
                    if (index < moneyCount) {
                        $(".btn-addFeetype").UnBindAndBind("click", function () {
                            //判断删除盒子里是否有序号
                            if (deleteFeeBox.length > 0) {
                                index = deleteFeeBox.shift();
                            }
                            BuildFeeTypeHtml(index);
                        });
                    } else {
                        t.msgwarn("最多只能添加{0}个".Format(moneyCount));
                        //最后一个删除选项进行初始化
                        deleteNewFeeType(index - 1);
                    }
                }

                function deleteNewFeeType(index) {
                    $(".btn-removeFeeType-{0}".Format(index)).UnBindAndBind("click", function () {
                        var lindex = AlertConfirm("确定要删除此票种吗？", "删除", "取消", function () {
                            CLOSE(lindex);
                            $(".feeitemwrap-{0}".Format(index)).remove();
                            deleteFeeBox.push(index);
                            var feeitemWrap = $(".feeitemwrap");
                            //if (feeitemWrap.length > 1) {
                            var feeitemwrapFirst = feeitemWrap.eq(0);
                            feeitemwrapFirst.removeClass("col-md-offset-2");
                            //判断删除时如何已达到满值减1，则最后一项显示添加票种按钮
                            //var feeitems = $(".feeitemwrap");
                            var _allLength = feeitemWrap.length;
                            var lastItem = feeitemWrap.last();
                            var btnAdd;
                            if (lastItem.find(".btn-addFeetype").length == 0) {
                                var btn = lastItem.find(".btn-removeFeeType"), btnIndex = btn.data("i");
                                btnAdd = $("<a>").addClass("btn btn-success btn-addFeetype".Format(index - 1)).data("i", btnIndex).text("+添加票种");
                                btn.after(btnAdd);
                            }
                            if (feeitemWrap.length == 1) {
                                lastItem.find(".btn-removeFeeType").remove();
                            }
                            //初始化添加按钮事件
                            addNewFeeType(_allLength);
                        });
                    });
                }

                function deleteNewJoinInfo(index) {
                    $(".btn-removeJoinInfo-{0}".Format(index)).UnBindAndBind("click", function () {
                        var lindex = AlertConfirm("确定要删除此填写项吗？", "删除", "取消", function () {
                            CLOSE(lindex);
                            $(".joinItemWrap-{0}".Format(index)).remove();
                            deleteJoinBox.push(index);
                            if ($(".joinItemWrap").length > 0) {
                                var joinitems = $(".joinItemWrap");
                                var _allLength = joinitems.length;
                                var lastItem = joinitems.last();
                                var btnAdd;
                                if (lastItem.find(".btn-addJoinInfo").length == 0) {
                                    var btn = lastItem.find(".btn-removeJoinInfo"), btnIndex = btn.data("i");
                                    btnAdd = $("<a>").addClass("btn btn-success btn-addJoinInfo".Format(index - 1)).data("i", btnIndex).text("+添加购买填写项")
                                    btn.after(btnAdd);
                                }
                                //初始化添加按钮事件
                                addJoinInfo(_allLength);
                            } else {
                                $('.joinItemsWrap').children().eq(1).last().append($("<a>").addClass("btn btn-success btn-addJoinInfo").text('+添加购买填写项'));
                                deleteJoinBox.length = 0;
                                addJoinInfo(0);
                            }
                        });
                    });
                }

                function BuildFeeTypeHtml(index) {
                    var totalCount = $(".feeitemwrap").length;
                    var html = [];
                    html.push('<div class="feeitemwrap feeitemwrap-{0} col-md-offset-2 col-md-10 Lpdl0 Lmgt5">'.Format(index));
                    html.push('<div class="col-md-9">');
                    html.push('<div class="col-md-3 col-sm-3 Lpdl0i">');
                    html.push('<input data-val="true" data-val-number="The field FeeType must be a number." id="PartyFee_{0}__FeeType" name="PartyFee[{0}].FeeType" type="hidden" value="">');
                    html.push('<select id="pickupFee{0}" class="form-control">');
                    html.push('<option value="">费用类型</option>');
                    html.push('<option value="0">免费</option>');
                    html.push('<option value="10">积分支付</option>');
                    html.push('<option value="20">VIP分支付</option>');
                    html.push('<option value="30">支付宝支付</option>');
                    html.push('</select>');
                    html.push('</div>');
                    html.push('<div class="col-md-3 col-sm-3 Lpdl0i">');
                    html.push('<input class="form-control" data-val="true" id="PartyFee_{0}__FeeName" min="0" name="PartyFee[{0}].FeeName" placeholder="票种名称">');
                    html.push('</div>');
                    html.push('<div class="col-md-3 col-sm-3 Lpdl0i">');
                    html.push('<input class="form-control" id="PartyFee_{0}__Fee" min="0" name="PartyFee[{0}].Fee" placeholder="报名费" type="number" value="">');
                    html.push('</div>');
                    html.push('<div class="col-md-3 col-sm-3 Lpdl0i">');
                    html.push('<input class="form-control" data-val="true" data-val-number="The field FeeCount must be a number." id="PartyFee_{0}__FeeCount" min="1" name="PartyFee[{0}].FeeCount" placeholder="活动名额" type="number" value="">');
                    html.push('</div>');
                    html.push('</div>');
                    html.push('<div class="col-md-3">');
                    html.push('<a class="btn btn-danger btn-removeFeeType btn-removeFeeType-{0}" data-i="{0}">- 删除票种</a>'.Format(index));
                    if (totalCount < moneyCount - 1) {
                        html.push('<a class="btn btn-success btn-addFeetype" data-i="{0}">+添加票种</a>'.Format(index));
                    }
                    html.push('</div>');
                    html.push('</div>');
                    var btnAdd = $(".btn-addFeetype"), btnIndex = btnAdd.data('i') != index ? btnAdd.data('i') : index - 1;
                    if (totalCount == 1) {
                        btnAdd.before($("<a>").addClass("btn btn-danger btn-removeFeeType btn-removeFeeType-{0}".Format(btnIndex)).data("i", btnIndex).text("- 删除票种".Format(btnIndex)));
                    }
                    btnAdd.unbind("click").remove();
                    $(".feetypeList").append(html.join("").Format(index));
                    if (btnIndex != index) {
                        deleteNewFeeType(index);
                    }
                    deleteNewFeeType(btnIndex);
                    addNewFeeType(totalCount + 1);
                    InitFeeTypeChange(index);
                }

                function addJoinInfo(index) {
                    if (index < maxItem) {
                        $(".btn-addJoinInfo").UnBindAndBind("click", function () {
                            if (deleteJoinBox.length > 0) {
                                index = deleteJoinBox.shift();
                            }
                            BuildJoinInfoHtml(index);
                        });
                    } else {
                        t.msgwarn("最多只能添加{0}个".Format(maxItem));
                        deleteNewJoinInfo(index - 1);
                    }
                }

                function BuildJoinInfoHtml(index) {
                    var totalCount = $(".joinItemWrap").length;
                    var html = [];
                    html.push('<div class="joinItemWrap joinItemWrap-{0} col-md-offset-2 col-md-10 Lpdl0 Lmgt5">'.Format(index));
                    html.push('<div class="col-md-3">');
                    html.push('<input id="JoinItemQues_{0}__ItemName" name="JoinItemQues[{0}].ItemName" class="form-control" type="text" placeholder="报名内容项名称">'.Format(index));
                    html.push('</div>');
                    html.push('<div class="col-md-9">');
                    html.push('<a class="btn btn-danger btn-removeJoinInfo btn-removeJoinInfo-{0}" data-i="{0}">- 删除购买填写项</a>'.Format(index));
                    if (totalCount < maxItem - 1) {
                        html.push('<a class="btn btn-success btn-addJoinInfo">+添加填写项</a>');
                    }
                    html.push('</div>');
                    html.push('</div>');


                    var btnAdd = $(".btn-addJoinInfo"), btnIndex = btnAdd.data('i') != index ? btnAdd.data('i') : index - 1;
                    btnAdd.unbind("click").remove();
                    $(".joinBaseInfos").append(html.join("").Format(index));
                    if (btnIndex != index) {
                        deleteNewJoinInfo(index);
                    }
                    deleteNewJoinInfo(btnIndex);
                    addJoinInfo(totalCount + 1);
                }

                InitFeeTypeChange(0);
                addNewFeeType(1);
                addJoinInfo(0);

                $("#Party_Title").focus();
            },
            edit: function (p) {
                var maxItem = 100, moneyCount = 100, deleteFeeBox = [], deleteJoinBox = [];
                var regu = new RegExp("^([0-9]*[.1-9])$"); //小数测试
                var reg = new RegExp("^[0-9]*[1-9][0-9]*$");
                $("#btn_EditParty").click(function () {
                    if (checkEmpty("#Party_Title", "活动主题")) { return !1; }
                    if (checkEmpty("#Party_ActivityIMG", "活动海报")) { $(window).scrollTop(0); $("#btn_addActiveImg").click(); return !1; }
                    //if (checkEmpty("#Party_BeginTime", "活动开始时间")) { return !1; }
                    //if (checkEmpty("#Party_EndTime", "活动结束时间")) { return !1; }
                    if ((checkEmpty("#SelectBeginTime", "活动开始时间"))) { return !1; }
                    if (!$("#Party_CanJoinOnBeginCHeck").is(":checked")) {
                        //if (checkEmpty("#Party_JoinBeginTime", "报名开始时间")) { return !1; }
                        //if ((checkEmpty("#Party_JoinEndTime", "报名截止时间"))) { return !1; }
                        if (checkEmpty("#SelectJoinTime", "活动报名时间")) { return !1; }
                    }
                    if (checkEmpty("#Party_Address", "活动地址")) { return !1; }
                    if ($('#summernote').summernote("isEmpty")) { layer.msg("活动详情不能为空"); return !1; }
                    else {
                        var feeok = true, saveIndexItem = [], maxIndex = 0;
                        for (var i = 0; i < moneyCount; i++) {
                            if ($(".feeitemwrap-{0}".Format(i)).length > 0) {
                                if (checkEmpty("#pickupFee{0}".Format(i), "费用类型")) { feeok = false; return !1; }
                                if (checkEmpty("#PartyFee_{0}__FeeName".Format(i), "票种名称")) { feeok = false; return !1; }
                                if (t.tools.CheckLength($("#PartyFee_{0}__FeeName".Format(i)).val(), 1, 15, "票种名称")) {
                                    if (checkEmpty("#PartyFee_{0}__Fee".Format(i), "报名费")) { feeok = false; return !1; }
                                    if (checkEmpty("#PartyFee_{0}__FeeCount".Format(i), "活动名额")) { feeok = false; return !1; }
                                    if ($("#pickupFee{0}".Format(i)).val() != 0 && $("#PartyFee_{0}__Fee".Format(i)).val() == 0) { $("#pickupFee{0}".Format(i)).focus(); Leo.msgfail("费用为0的话，请选择免费"); feeok = false; return !1; }
                                } else {
                                    $("#PartyFee_{0}__FeeName".Format(i)).focus();
                                    feeok = false;
                                    return !1;
                                }
                                maxIndex = i;
                            } else {
                                saveIndexItem.push(i);
                            }
                        }
                        if (feeok) {
                            $.each(saveIndexItem, function (i, n) {
                                if (n < maxIndex) {
                                    var html = [];
                                    html.push('<div class="feeitemwrap feeitemwrap-{0} col-md-10 Lpdl0 Ldn">'.Format(n));
                                    html.push('<input data-val="true" data-val-number="The field FeeType must be a number." id="PartyFee_{0}__FeeType" name="PartyFee[{0}].FeeType" type="hidden" value="">'.Format(n));
                                    html.push('<input class="form-control" id="PartyFee_{0}__FeeName" name="PartyFee[{0}].FeeName" placeholder="票种名称" type="text" value="">'.Format(n));
                                    html.push('<input class="form-control" data-val="true" data-val-number="The field Fee must be a number." id="PartyFee_{0}__Fee" min="0" name="PartyFee[{0}].Fee" placeholder="报名费" type="number" value="">'.Format(n));
                                    html.push('<input class="form-control" data-val="true" data-val-number="The field FeeCount must be a number." id="PartyFee_{0}__FeeCount" min="1" name="PartyFee[{0}].FeeCount" placeholder="活动名额" type="number" value="">'.Format(n));
                                    html.push("</div>");
                                    $(".feetypeList").append(html.join(''));
                                }
                            });
                            var joinSaveIndexItem = [], maxJoinIndex = 0;
                            for (var i = 0; i < maxItem; i++) {
                                var joinItem = "#JoinItemQues_{0}__ItemName".Format(i);
                                if ($(joinItem).length > 0) {
                                    if (checkEmpty(joinItem, "报名项名称")) {
                                        feeok = false; return !1;
                                    } else {
                                        feeok = t.tools.CheckLength(joinItem.val(), 1, 10, "报名项名称");
                                    }
                                    if (feeok) {
                                        maxJoinIndex = i;
                                    }
                                } else {
                                    joinSaveIndexItem.push(i);
                                }
                            }
                            if (feeok) {
                                $.each(joinSaveIndexItem, function (i, n) {
                                    if (n < maxJoinIndex) {
                                        var html = [];
                                        html.push('<div class="joinItemWrap joinItemWrap-{0} col-md-offset-2 col-md-10 Lpdl0 Lmgt5 Ldn">'.Format(n));
                                        html.push('<input id="JoinItemQues_{0}__ItemName" name="JoinItemQues[{0}].ItemName" class="form-control" type="hidden" placeholder="报名项名称">'.Format(n));
                                        html.push("</div>");
                                        $(".joinBaseInfos").append(html.join(''));
                                    }
                                });
                                ////数据拆分
                                if ($("#Party_CanJoinOnBeginCHeck").is(":checked")) {
                                    $("#Party_CanJoinOnBegin").val(1);
                                } else {
                                    $("#Party_CanJoinOnBegin").val(2);
                                    //包装报名时间
                                    var jointimes = $("#SelectJoinTime").val().split(" - ");
                                    $("#Party_JoinBeginTime").val(jointimes[0]);
                                    $("#Party_JoinEndTime").val(jointimes[1]);
                                }
                                //包装活动时间
                                var jointimes = $("#SelectBeginTime").val().split(" - ");
                                $("#Party_BeginTime").val(jointimes[0]);
                                $("#Party_EndTime").val(jointimes[1]);
                                //////////////////
                                $("#Party_Body").val(encodeURI($('#summernote').summernote("code")));
                                var lindex = LOAD("正在修改活动数据...");
                                $("#nmform").ajaxSubmit(function (data) {
                                    if (data.Ok) {
                                        t.msgsuccess(data.Msg, function () { location.href = "/party/detail/{0}".Format(p) });
                                    } else {
                                        t.msgfail(data.Msg || "编辑失败");
                                    }
                                    CLOSE(lindex);
                                });
                            }
                        }
                    }
                });
                $("#Party_CanJoinOnBeginCHeck").change(function () {
                    if (this.checked) {
                        $(".setJoinTime").addClass("Ldn");
                    } else {
                        $(".setJoinTime").removeClass("Ldn");
                    }
                });
                $("#pickupFee").change(function () {
                    $(this).children().length == 5 && $(this).children().eq(0).remove();
                    $("#PartyFee_0__FeeType").val(this.value);
                    if (this.value == "0") {
                        $("#PartyFee_0__Fee").val("");
                        $(".PartyFee").addClass("Ldn");
                    } else {
                        $(".PartyFee").removeClass("Ldn");
                        $("#PartyFee_0__Fee").val("");
                    }
                });

                var lindex;
                $("#btn_addActiveImg").click(function () {
                    if ($("#ActivityIMGS .leo").children().length == 1) {
                        var imgs = ["juhui", "lvyou", "shangwu", "qinzi", "wenhua", "yundong", "yanchu"];
                        var html = [];
                        $.each(imgs, function (i, n) {
                            for (var i = 1; i < 13; i++) {
                                var divEle = $("<div>").addClass("imgwrap col-sm-6 col-md-4");
                                var aEle = $("<a>").addClass("thumbnail");
                                var imgEle = $("<img>").attr({ "title": "点击选择", "src": "/Content/img/party/poster_image_{0}_{1}.jpg".Format(n, i) }).click(function () {
                                    var src = $(this).attr("src");
                                    $("#Party_ActivityIMG").val(src);
                                    $("#btn_addActiveImg").prev().attr("src", src);
                                    layer.close(lindex);
                                });
                                $("#ActivityIMGS .leo").append(divEle.append(aEle.append(imgEle)));
                            }
                        });
                        $("#ActivityIMGS .leo").append(html.join(""));
                    }

                    lindex = layer.open({
                        scrollbar: !1,
                        type: 1,
                        title: false,
                        closeBtn: 0,
                        area: ["620px", "410px"],
                        shadeClose: !0,//单击遮罩层关闭窗口
                        content: $("#ActivityIMGS")

                    })
                });

                function PICKUPIMG(target) {
                    var src = $(target).attr("src");
                    $("#Party_ActivityIMG").val(src);
                    $("#btn_addActiveImg").prev().attr("src", src);
                    layer.close(lindex);
                }

                function checkEmpty($input, msg) {
                    if (Leo.tools.isEmptyObject($.trim($($input).val()))) {
                        $($input).focus();
                        layer.msg((msg || "") + "不能为空");
                        return true;
                    }
                    else {
                        return false;
                    }
                }

                function InitLayDateCSS(wrapElement) {
                    var browser = t.browser;
                    if (browser.versions.ios || browser.versions.android || browser.versions.iPhone) {
                        var laydate = $(wrapElement);
                        laydate.css({ "font-size": "12px", "width": "100%", "left": "0" });
                        laydate.find(".layui-laydate-main").css("width", "50%");
                        var laydatelist0 = laydate.find(".laydate-main-list-0");
                        var laydatelist1 = laydate.find(".laydate-main-list-1");
                        laydatelist0.find(".layui-laydate-header").css("padding-right", 0).find(".laydate-set-ym").css("text-align", "left");
                        laydatelist0.find(".layui-laydate-content").css("padding", 0);
                        laydatelist1.find(".layui-laydate-header").css("padding-left", 0).find(".laydate-set-ym").css("text-align", "right");
                        laydatelist1.find(".layui-laydate-content").css("padding", 0);
                    }
                }

                $.each(["#SelectBeginTime", "#SelectJoinTime"], function (i, n) {
                    laydate.render({
                        elem: n
                        , type: 'datetime'
                        , range: true
                        , trigger: 'click'
                        , min: new Date().getFullYear().toString() + "-" + (new Date().getMonth() + 1).toString() + "-" + new Date().getDate().toString()
                        , ready: function (index) {
                            InitLayDateCSS("#layui-laydate1");
                        }
                        , done: function (value, date) {
                            if (n == "#SelectJoinTime") {
                                var _starttime = $("#SelectBeginTime");
                                if (t.tools.CheckFormNotEmpty(_starttime, "请先选择活动时间！", !1)) {
                                    if (t.tools.DateCompare(_starttime.val().split(" - ")[0], value.split(" - ")[1]) == 1) {
                                        t.msgfail("活动报名截止时间必须在活动开始时间之前！");
                                        $("#SelectJoinTime").focus();
                                        setTimeout(function () { $("#SelectJoinTime").val(""); }, 1);
                                    }
                                } else {
                                    setTimeout(function () { $("#SelectJoinTime").val(""); }, 1);
                                }
                            }
                        }
                    });
                });

                Leo.RichText("#summernote", "/tool/uploadimg", "请输入活动内容", 300, !1, function (data) {
                    $('#summernote').summernote('insertImage', data.Url);
                }, null, null, $("#Party_Body").val());

                //以下为票种及报名填写项
                function InitFeeTypeChange(index) {
                    $("#pickupFee{0}".Format(index)).change(function () {
                        $(this).children().length == 5 && $(this).children().eq(0).remove();
                        $("#PartyFee_{0}__FeeType".Format(index)).val(this.value);
                        if (this.value == "0") {
                            $("#PartyFee_{0}__Fee".Format(index)).attr("readonly", !0).val("0");
                            $("#PartyFee_{0}__FeeName".Format(index)).val("福利票");
                        } else {
                            $("#PartyFee_{0}__Fee".Format(index)).removeAttr("readonly").val("");
                            $("#PartyFee_{0}__FeeName".Format(index)).val("");
                        }
                    });

                    $("#PartyFee_{0}__Fee".Format(index)).on("blur change paste", function () {
                        var item = $("#PartyFee_{0}__FeeType".Format(index)), me = $(this);
                        if (item.val() !== "30") {
                            var v = me.val();
                            if (!Leo.tools.isEmptyObject(v)) {
                                var result = parseFloat(v);
                                if (!reg.test(result)) {
                                    Leo.msgfail("非现金支付时不能为小数，只能为整数！");
                                    me.css("border-color", "red").focus();
                                }
                            }
                        }
                    });
                }

                function addNewFeeType(index) {
                    if (index < moneyCount) {
                        $(".btn-addFeetype").UnBindAndBind("click", function () {
                            //判断删除盒子里是否有序号
                            if (deleteFeeBox.length > 0) {
                                index = deleteFeeBox.shift();
                            }
                            BuildFeeTypeHtml(index);
                        });
                    } else {
                        t.msgwarn("最多只能添加{0}个".Format(moneyCount));
                        //最后一个删除选项进行初始化
                        deleteNewFeeType(index - 1);
                    }
                }

                function deleteNewFeeType(index) {
                    $(".btn-removeFeeType-{0}".Format(index)).UnBindAndBind("click", function () {
                        var lindex = AlertConfirm("确定要删除此票种吗？", "删除", "取消", function () {
                            CLOSE(lindex);
                            $(".feeitemwrap-{0}".Format(index)).remove();
                            deleteFeeBox.push(index);
                            var feeitemWrap = $(".feeitemwrap");
                            //if (feeitemWrap.length > 1) {
                            var feeitemwrapFirst = feeitemWrap.eq(0);
                            feeitemwrapFirst.removeClass("col-md-offset-2");
                            //判断删除时如何已达到满值减1，则最后一项显示添加票种按钮
                            //var feeitems = $(".feeitemwrap");
                            var _allLength = feeitemWrap.length;
                            var lastItem = feeitemWrap.last();
                            var btnAdd;
                            if (lastItem.find(".btn-addFeetype").length == 0) {
                                var btn = lastItem.find(".btn-removeFeeType"), btnIndex = btn.data("i");
                                btnAdd = $("<a>").addClass("btn btn-success btn-addFeetype".Format(index - 1)).data("i", btnIndex).text("+添加票种");
                                btn.after(btnAdd);
                            }
                            if (feeitemWrap.length == 1) {
                                lastItem.find(".btn-removeFeeType").remove();
                            }
                            //初始化添加按钮事件
                            addNewFeeType(_allLength);
                        });
                    });
                }

                function deleteNewJoinInfo(index) {
                    $(".btn-removeJoinInfo-{0}".Format(index)).UnBindAndBind("click", function () {
                        var lindex = AlertConfirm("确定要删除此填写项吗？", "删除", "取消", function () {
                            CLOSE(lindex);
                            $(".joinItemWrap-{0}".Format(index)).remove();
                            deleteJoinBox.push(index);
                            if ($(".joinItemWrap").length > 0) {
                                var joinitems = $(".joinItemWrap");
                                var _allLength = joinitems.length;
                                var lastItem = joinitems.last();
                                var btnAdd;
                                if (lastItem.find(".btn-addJoinInfo").length == 0) {
                                    var btn = lastItem.find(".btn-removeJoinInfo"), btnIndex = btn.data("i");
                                    btnAdd = $("<a>").addClass("btn btn-success btn-addJoinInfo".Format(index - 1)).data("i", btnIndex).text("+添加报名填写项")
                                    btn.after(btnAdd);
                                }
                                //初始化添加按钮事件
                                addJoinInfo(_allLength);
                            } else {
                                $('.joinItemsWrap').children().eq(1).last().append($("<a>").addClass("btn btn-success btn-addJoinInfo").text('+添加报名填写项'));
                                deleteJoinBox.length = 0;
                                addJoinInfo(0);
                            }
                        });
                    });
                }

                function BuildFeeTypeHtml(index) {
                    var totalCount = $(".feeitemwrap").length;
                    var html = [];
                    html.push('<div class="feeitemwrap feeitemwrap-{0} col-md-offset-2 col-md-10 Lpdl0 Lmgt5">'.Format(index));
                    html.push('<div class="col-md-9">');
                    html.push('<div class="col-md-3 col-sm-3 Lpdl0i">');
                    html.push('<input data-val="true" data-val-number="The field FeeType must be a number." id="PartyFee_{0}__FeeType" name="PartyFee[{0}].FeeType" type="hidden" value="">');
                    html.push('<select id="pickupFee{0}" class="form-control">');
                    html.push('<option value="">费用类型</option>');
                    html.push('<option value="0">免费</option>');
                    html.push('<option value="10">积分支付</option>');
                    html.push('<option value="20">VIP分支付</option>');
                    html.push('<option value="30">支付宝支付</option>');
                    html.push('</select>');
                    html.push('</div>');
                    html.push('<div class="col-md-3 col-sm-3 Lpdl0i">');
                    html.push('<input class="form-control" data-val="true" id="PartyFee_{0}__FeeName" min="0" name="PartyFee[{0}].FeeName" placeholder="票种名称">');
                    html.push('</div>');
                    html.push('<div class="col-md-3 col-sm-3 Lpdl0i">');
                    html.push('<input class="form-control" id="PartyFee_{0}__Fee" min="0" name="PartyFee[{0}].Fee" placeholder="报名费" type="number" value="">');
                    html.push('</div>');
                    html.push('<div class="col-md-3 col-sm-3 Lpdl0i">');
                    html.push('<input class="form-control" data-val="true" data-val-number="The field FeeCount must be a number." id="PartyFee_{0}__FeeCount" min="1" name="PartyFee[{0}].FeeCount" placeholder="活动名额" type="number" value="">');
                    html.push('</div>');
                    html.push('</div>');
                    html.push('<div class="col-md-3">');
                    html.push('<a class="btn btn-danger btn-removeFeeType btn-removeFeeType-{0}" data-i="{0}">- 删除票种</a>'.Format(index));
                    if (totalCount < moneyCount - 1) {
                        html.push('<a class="btn btn-success btn-addFeetype" data-i="{0}">+添加票种</a>'.Format(index));
                    }
                    html.push('</div>');
                    html.push('</div>');
                    var btnAdd = $(".btn-addFeetype"), btnIndex = btnAdd.data('i') != index ? btnAdd.data('i') : index - 1;
                    if (totalCount == 1) {
                        btnAdd.before($("<a>").addClass("btn btn-danger btn-removeFeeType btn-removeFeeType-{0}".Format(btnIndex)).data("i", btnIndex).text("- 删除票种".Format(btnIndex)));
                    }
                    btnAdd.unbind("click").remove();
                    $(".feetypeList").append(html.join("").Format(index));
                    if (btnIndex != index) {
                        deleteNewFeeType(index);
                    }
                    deleteNewFeeType(btnIndex);
                    addNewFeeType(index + 1);
                    InitFeeTypeChange(index);
                }

                function addJoinInfo(index) {
                    if (index < maxItem) {
                        $(".btn-addJoinInfo").UnBindAndBind("click", function () {
                            if (deleteJoinBox.length > 0) {
                                index = deleteJoinBox.shift();
                            }
                            BuildJoinInfoHtml(index);
                        });
                    } else {
                        t.msgwarn("最多只能添加{0}个".Format(maxItem));
                        deleteNewJoinInfo(index - 1);
                    }
                }

                function BuildJoinInfoHtml(index) {
                    var totalCount = $(".joinItemWrap").length;
                    var html = [];
                    html.push('<div class="joinItemWrap joinItemWrap-{0} col-md-offset-2 col-md-10 Lpdl0 Lmgt5">'.Format(index));
                    html.push('<div class="col-md-3">');
                    html.push('<input id="JoinItemQues_{0}__ItemName" name="JoinItemQues[{0}].ItemName" class="form-control" type="text" placeholder="报名内容项名称">'.Format(index));
                    html.push('</div>');
                    html.push('<div class="col-md-9">');
                    html.push('<a class="btn btn-danger btn-removeJoinInfo btn-removeJoinInfo-{0}" data-i="{0}">- 删除购买填写项</a>'.Format(index));
                    if (totalCount < maxItem - 1) {
                        html.push('<a class="btn btn-success btn-addJoinInfo" data-i="{0}">+添加填写项</a>'.Format(index));
                    }
                    html.push('</div>');
                    html.push('</div>');
                    var btnAdd = $(".btn-addJoinInfo"), btnIndex = btnAdd.data('i') != index ? btnAdd.data('i') : index - 1;
                    btnAdd.unbind("click").remove();
                    $(".joinBaseInfos").append(html.join("").Format(index));
                    if (btnIndex != index) {
                        deleteNewJoinInfo(index);
                    }
                    deleteNewJoinInfo(btnIndex);
                    addJoinInfo(totalCount + 1);
                }

                var feeitemwrapcount = $(".feetypeList").find(".feeitemwrap").length;
                if (feeitemwrapcount < moneyCount) {
                    addNewFeeType(feeitemwrapcount);
                }
                for (var __i = 0; __i < feeitemwrapcount; __i++) {
                    InitFeeTypeChange(__i);
                }
                var joincount = $(".joinBaseInfos").find(".joinItemWrap").length;
                joincount < maxItem && addJoinInfo(joincount);

                $("#Party_Title").focus();
            },
        };
        return function (action, p) {
            return T[action](p);
        }
    },
    gift: function () {
        var t = this,
            T = {
                ReSizeIndexPic: function () {
                    var items = $(".giftItem");
                    if (items.length > 0) {
                        var gwidth = items.eq(0).width();
                        $.each(items, function () {
                            $(this).find("a div").eq(0).height(gwidth / 1.5);
                        });
                    }
                },
                index: function (p) {
                    var me = T;
                    t.Search(function (keyword) {
                        var layerIndex = LOAD("正在搜索...");
                        $.get("/gift/search", { key: keyword, id: location.pathname.split("/")[3] || 1 }, function (data) {
                            if (data.Ok === false) {
                                layer.msg(data.Msg);
                            } else {
                                $(".gift_list").html(data);
                                me.ReSizeIndexPic();
                            }
                            CLOSE(layerIndex);
                        });
                    });
                    me.ReSizeIndexPic();
                    t.BootStrap_Tab_Change(".gift_list", ".gift_list", ".gift_Page_Wrap", "/gift/loadmore/{0}".Format(p), null, null, null, function () {
                        me.ReSizeIndexPic();
                    });
                },
                detail: function (p) {
                    var _btnBuy = $(".btn_Ibuy");
                    _btnBuy.data("p", p);
                    var btype = _btnBuy.data("type");
                    _btnBuy.UnBindAndBind("click", function () {
                        var me = $(this), _feeselected = $(".detail_join_ticket_con .ticket li.select table .ticket_money");
                        if (_feeselected.length > 0) {
                            var _fee = _feeselected.data("fee"), _feeIden = _feeselected.data("feeidn"), _feetype = _feeselected.data("feetype"), _msg = _fee == 0 ? "购买免费" : "{0}".Format(_fee), mustWriteJoinItem = $(".baseInfoToJoin").length > 0, buycount = $(".select_num .count input").val(), hasBuy = me.hasClass("hasBuy"), buymsgtip = "温馨提示：";
                            if (hasBuy) {
                                buymsgtip = "您已经购买";
                                var buyitems = me.data("joinitem");
                                $.each(buyitems, function (i, n) {
                                    buymsgtip += "{0}份【{1}】票".Format(buyitems[i].count, buyitems[i].name);
                                    if (i < buyitems.length - 1) {
                                        buymsgtip += "，";
                                    }
                                });
                            }
                            //////////////////////
                            var _desc = _feetype == 0 ? "{0}" : _feetype == 10 ? "积分付费{0}" : _feetype == 20 ? "VIP分付费{0}" : "人民币付费{0}元";
                            var layerIndex = AlertConfirm("{2}<br/>您本次购买信息如下：<br>数量：{0}份<br>付费类型：{1}<br>你确定要购买吗？".Format(buycount, _desc.Format(_fee == 0 ? _msg : "<br>总费用：{0}".Format(_fee * buycount)), buymsgtip), mustWriteJoinItem ? "填资料" : "确定购买", "我再想想",
                                function () {
                                    //获取该用户的历史记录,获取个人信息
                                    $.get("/gift/userbuyinfo/" + me.data("aid"), function (res) {
                                        if (res.Ok) {
                                            !$(".joinInputForm1").val() && $(".joinInputForm1").val(res.Data.uinfo ? res.Data.uinfo.name : "");
                                            !$(".joinInputForm2").val() && $(".joinInputForm2").val(res.Data.uinfo ? res.Data.uinfo.tel : "");
                                            $.each(res.Data.ext, function () {
                                                !$(".complete_" + this.id).val() && $(".complete_" + this.id).val(this.answer || "");
                                            });
                                        }
                                    });
                                    CLOSE(layerIndex);
                                    var par = { fee: parseFloat(_fee), feeid: _feeIden, count: buycount };
                                    if (mustWriteJoinItem) {
                                        var h = $(".btn-subInfoAndJoin").data("height");
                                        layerIndex = AlertDiv_End(".baseInfoToJoin", $(window).width() > 418 ? "418px" : "370px", h || "200px", "填写购买基本资料", null, function () { });
                                        $(".btn-subInfoAndJoin").UnBindAndBind("click", function () {
                                            var wok = !1, joinitems = $(".joinInputForm"), joinItemcount = joinitems.length;
                                            par.JoinItems = [];
                                            for (var i = 1; i <= joinItemcount; i++) {
                                                var item = $(".joinInputForm{0}".Format(i));
                                                if (t.tools.CheckFormNotEmpty(item, item.attr("placeholder"))) {
                                                    if (t.tools.CheckLength(item.val(), 1, 100, item.attr("placeholder"))) {
                                                        wok = !0;
                                                        i < 3 ? par["{0}".Format(i == 1 ? "LinkMan" : "LinkTel")] = item.val() : par.JoinItems.push({ Id: item.data("id"), Value: item.val() })
                                                    } else {
                                                        par.JoinItems = [];
                                                        return;
                                                    }
                                                } else {
                                                    par.JoinItems = [];
                                                    return;
                                                }
                                            }
                                            wok && (BuyAction(me.data("aid"), _feetype, par));
                                        });
                                    } else {
                                        BuyAction(me.data("aid"), _feetype, par);
                                    }
                                }
                            );
                        } else {
                            t.msgfail("请先选择任一票种类型进行购买！");
                        }
                    });

                    function BuyAction(mid, feetype, par) {
                        feetype != 30 ? (function () {
                            var layerMask = LOAD("正在购买...");
                            par.desc = "百晓堂购买商品" + mid;
                            $.post("/gift/submit/{1}?t={0}".Format(Leo.getPK(), mid), par, function (data) {
                                if (data.Ok) {
                                    layer.closeAll();
                                    //$(".baseInfoToJoin").remove();
                                    //$(".btn_Ibuy").text("您已购买").unbind("click").removeClass("btn_Ibuy");
                                    //t.msgsuccess(data.Msg || "购买成功", function () { layer.close(layerMask); location.reload(); }, 500);
                                    $(".baseInfoToJoin").find(".joinInputForm").val("");
                                    t.MsgTips("恭喜您购买成功", (data.Msg || "购买成功") + (data.Data ? ("<br>" + data.Data) + "<br>可在<a href='{0}/user/notice' target='_blank'>通知</a>里查看".Format(t.baseUrl) : ""));
                                    var selectTickets = $(".tickets li.select"), numEle = selectTickets.find(".num"), nowCount = parseInt(numEle.data("count")) - par.count;
                                    if (nowCount <= 0) {
                                        selectTickets.removeClass("select").addClass("gray end");
                                        numEle.data("count", 0).text("剩余0张");
                                    } else {
                                        numEle.data("count", nowCount).text("剩余{0}张".Format(nowCount));
                                        $(".select_num .item_limit_note").text("剩余{0}张".Format(nowCount));
                                    }
                                    var noticeBox = $(".bxtnoticebox");
                                    //noticeBox.removeClass("Ldni").find(".noticecount").text(1);
                                    noticeBox.find(".noticecount").text(1);
                                } else {
                                    t.msgfail(data.Msg, function () {
                                        if (data.Type == 1) {
                                            location.reload(!0);
                                        }
                                    });
                                }
                            });
                        })() : (function () {
                            var layerOrder = LOAD("正在生成订单...");
                            par.type = 2;
                            par.id = par.feeid;
                            par.desc = "百晓堂购买商品" + mid;
                            par.mid = mid;
                            setTimeout(function () {
                                $.post("/Pay/Pay", par, function (data) {
                                    if (data.Ok) {
                                        location.href = data.Url + "?t=" + t.getPK();
                                    } else {
                                        layer.closeAll();
                                        t.msgfail(data.Msg, function () {
                                            if (data.Type == 1) {
                                                location.reload(!0);
                                            }
                                        });
                                    }
                                });
                            }, t.tools.Random(500, 1000));
                        })();
                    }
                    //选择票种
                    $(".tickets li").click(function () {
                        var _me = $(this);
                        if (!_me.hasClass("gray end")) {
                            $(".tickets li.select").removeClass("select");
                            _me.addClass("select");
                            $(".item_limit_note").html(_me.find("table").find(".num").text());
                            $('.select_num input').val(1);
                        }
                    }).mouseover(function () {
                        $(this).addClass("select_hover");
                    }).mouseleave(function () {
                        $(this).removeClass("select_hover");
                    });
                    //获取购买人数
                    $.get("/gift/getbuyusers/{0}".Format(p), function (data) {
                        if (data.Data != null && data.Data.length > 0) {
                            $(".titleBuyCount").text(data.Data.length);
                            var defaultUrl = "/Content/img/head_default.gif";
                            var wrap = $(".joinedUserList ul");
                            $.each(data.Data, function (i, n) {
                                wrap.append($("<li>").addClass("Lfl").append($("<a>").css({ display: "block" }).attr({ "data-toggle": "tooltip", title: n["UserName"], target: "_blank", "href": "/User/detail/{0}".Format(n["UserName"]) }).append($("<img>").attr({ "src": n["HeadUrl"] || defaultUrl, width: 48 }))));
                            });
                            t.ToolTip();
                        }
                    });
                    ~function () {
                        //加减数量
                        var verifyCount = function (onfocus) {
                            var selectCount = select_num.find("input"), countVal = parseInt(selectCount.val()) || (onfocus ? "" : 1),
                                totalCount = parseInt($(".tickets li.select .num").data("count") || 0);
                            if ((onfocus && countVal) || !onfocus) {
                                if (countVal >= totalCount) {
                                    countVal = totalCount;
                                    t.msgfail("余票只剩{0}张！".Format(totalCount));
                                } else {
                                    onfocus || countVal++;
                                }
                                selectCount.val(countVal);
                            }
                        }
                        var select_num = $('.select_num');
                        select_num.find("input").blur(function () {
                            var me = $(this), _v = me.val();
                            (!_v || !parseInt(_v)) && me.val(1);
                        }).on("paste keyup", function () {
                            verifyCount(!0);
                        });
                        select_num.find(".less").UnBindAndBind("click", function () {
                            var selectCount = select_num.find("input"), countVal = parseInt(selectCount.val()) || 1;
                            if (countVal > 1) {
                                selectCount.val(parseInt(countVal) - 1);
                            } else {
                                t.msgfail("已经是1张了，不能再低了！");
                                selectCount.val(1);
                            }
                        });
                        select_num.find(".add").UnBindAndBind("click", function () {
                            verifyCount(!1);
                        });
                    }();
                    //清除粘贴的表格的格式
                    $(".content-main").find("table").removeAttr("width").css("width", "100%");
                    $(document).resize(function () {
                        $(".content-main").find("table").removeAttr("width").css("width", "100%");
                    })
                },
            };
        return function (action, p) {
            return T[action](p);
        }
    },
    zhaopin: function () {
        var t = this,
            T = {
                common: function () {
                    var tool = t.tools, check = tool.CheckFormNotEmpty, checkLength = tool.CheckLength;
                    function InitInput() {
                        Leo.tools.PickupInput("#JobPic", "#jobPic-input");
                        $("input[name='WorkeType']").change(function () {
                            if ($("input[name='WorkeType']:checked").val() == "2") {
                                $("#jianzhi").removeClass("Ldn");
                            } else {
                                $("#jianzhi").addClass("Ldn");
                            }
                        });
                    }

                    function Sub(addorEdit, feeType) {
                        if (check("#CName", "公司名称")) {
                            if (checkLength("#CName".val(), 1, 15, "公司名称")) {
                                if (check("#CPeople", "公司人数")) {
                                    if (check("#BelongJobTrade", "行业")) {
                                        if (check("#BelongJob", "岗位小类")) {
                                            if (check("#Gangwei", "招聘岗位")) {
                                                if (checkLength("#Gangwei".val(), 1, 20, "招聘岗位")) {
                                                    if (check("#deadTime", "过期时间")) {
                                                        if (parseInt("#deadTime".val()) >= 15 && parseInt("#deadTime".val()) <= 45) {
                                                            if (check("#Money", "薪资待遇")) {
                                                                if (check("#Study", "学历要求")) {
                                                                    if (check("#WorkHistory", "工作经验")) {
                                                                        if (check("#NeedCount", "招聘人数")) {
                                                                            if (check("#WorkPlace", "工作地点")) {
                                                                                if (checkLength("#WorkPlace".val(), 1, 15, "工作地点")) {
                                                                                    if (check("#Contact", "联系方式")) {
                                                                                        if (checkLength($("#Contact").val(), 1, 50, "联系方式")) {
                                                                                            if (check("#CDesc", "公司简介")) {
                                                                                                if (checkLength($("#CDesc").val(), 1, 200, "公司简介")) {
                                                                                                    if (check("#JobFuLi", "职位福利")) {
                                                                                                        if (checkLength($("#JobFuLi").val(), 1, 200, "职位福利")) {
                                                                                                            if (check("#JobRequire", "职位要求")) {
                                                                                                                if (checkLength($("#JobRequire").val(), 1, 200, "职位要求")) {
                                                                                                                    //if (check("#jobPic-input", "职位介绍照片")) {
                                                                                                                    var layerIndex = LOAD(),
                                                                                                                        formdata = new FormData(),
                                                                                                                        desc;
                                                                                                                    if (addorEdit == 1) {
                                                                                                                        desc = "发布";
                                                                                                                        if (feeType == 1) {
                                                                                                                            $("#nmform").attr("action", "/pay/zhaopinpay");
                                                                                                                        }
                                                                                                                    } else {
                                                                                                                        desc = "编辑";
                                                                                                                        ($("#JobPic").data("ischanged") && (formdata.append("JobPic", $("#JobPic")[0].files[0]), formdata.append("ischange", true))), formdata.append("ZhaoPinID", $("#ZhaoPinID").val()), $("#nmform").attr("action", "/zhaopin/edit")
                                                                                                                    }
                                                                                                                    var hasPic = false;
                                                                                                                    !t.tools.isEmptyObject("#jobPic-input".val()) && (hasPic = true, formdata.append("JobPic", $("#JobPic")[0].files[0]));
                                                                                                                    formdata.append("hasPic", hasPic);
                                                                                                                    formdata.append("CName", $("#CName").val());
                                                                                                                    formdata.append("CPeople", $("#CPeople").val());
                                                                                                                    formdata.append("Gangwei", $("#Gangwei").val());
                                                                                                                    formdata.append("deadTime", $("#deadTime").val());
                                                                                                                    formdata.append("Money", $("#Money").val());
                                                                                                                    formdata.append("Study", $("#Study").val());
                                                                                                                    formdata.append("WorkHistory", $("#WorkHistory").val());
                                                                                                                    formdata.append("NeedCount", $("#NeedCount").val());
                                                                                                                    formdata.append("WorkPlace", $("#WorkPlace").val());
                                                                                                                    formdata.append("Contact", $("#Contact").val());
                                                                                                                    formdata.append("CDesc", $("#CDesc").val());
                                                                                                                    formdata.append("JobRequire", $("#JobRequire").val());
                                                                                                                    formdata.append("JobFuLi", $("#JobFuLi").val());
                                                                                                                    formdata.append("BelongJobTrade", $("#BelongJobTrade").val());
                                                                                                                    formdata.append("BelongJob", $("#BelongJob").val());
                                                                                                                    formdata.append("WorkeType", $("input[name='WorkeType']:checked").val());
                                                                                                                    if ($("input[name='WorkeType']:checked").val() == "2") {
                                                                                                                        formdata.append("WorkTime", "{0}_{1}_{2}_{3}_{4}".Format($("#SelectworkMonth").val(), $("#SelectworkWeek").val(), $("#SelectworkStartWeek").val(), $("#SelectworkEndWeek").val(), $("#SelectworkHour").val()));
                                                                                                                    }
                                                                                                                    $.ajax({
                                                                                                                        url: $("#nmform").attr("action"),
                                                                                                                        type: "post",
                                                                                                                        dataType: "json",
                                                                                                                        contentType: false,
                                                                                                                        processData: false,
                                                                                                                        data: formdata,
                                                                                                                        success: function (data) {
                                                                                                                            if (data.Ok) {
                                                                                                                                feeType == 1 ? t.msgsuccess("正在跳转支付宝支付") : (layer.msg("招聘信息{0}成功".Format(desc)));
                                                                                                                                location.href = data.Url;
                                                                                                                            } else {
                                                                                                                                layer.msg(data.Msg || "{0}失败".Format(desc));
                                                                                                                            }
                                                                                                                            CLOSE(layerIndex);
                                                                                                                        }
                                                                                                                    });
                                                                                                                    //}
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        } else {
                                                            $("#deadTime").focus();
                                                            t.msgfail("过期时间15~45天内");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return {
                        init: InitInput,
                        Submit: Sub
                    }
                }(),
                publish: function () {
                    T.common.init();
                    $("#btn_addZhaoPin").UnBindAndBind("click", function () {
                        T.common.Submit(1, 0);
                    });
                    $("#btn_addZhaoPinCash").UnBindAndBind("click", function () {
                        T.common.Submit(1, 1);
                    });
                },
                edit: function () {
                    T.common.init();
                    $("#btn_addProductCash").UnBindAndBind("click", function () {
                        T.common.Submit(2);
                    });
                },
                index: function () {
                    var check = t.tools.isEmptyObject;
                    Leo.BootStrap_Tab_Change(".job-searchForm", ".zhaopin-list-wrap", "#zhaopinPage", "/zhaopin/zhaopins");
                    t.Search(function (keyword) {
                        var layerIndex = LOAD("正在搜索...");
                        $.get("/zhaopin/search?key={0}".Format(keyword), function (data) {
                            if (data.Ok === false) {
                                layer.msg(data.Msg);
                            } else {
                                $(".zhaopin-list-wrap").html(data);
                                Leo.BootStrap_Tab_Change(".job-searchForm", ".zhaopin-list-wrap", "#zhaopinPage", "/zhaopin/search?key={0}", keyword);
                            }
                            CLOSE(layerIndex);
                        });
                    });
                    $(".btn-search-form").UnBindAndBind("click", function () {
                        var search_jobTrade = "#search_jobTrade", search_jobType = "#search_jobType",
                            search_job = "#search_job", search_companyName = "#search_companyName", search_workPlace = "#search_workPlace", search_money = "#search_money", search_study = "#search_study";
                        if (check(search_jobTrade.val()) && check(search_jobType.val()) && check(search_job.val()) && check(search_companyName.val()) && check(search_workPlace.val()) && check(search_money.val()) && check(search_study.val()) && check($("input[name='search_worktype']:checked").val())) {
                            t.msgfail("请进行任意筛选再进行搜索");
                            $(".job-searchForm").css("border", "1px solid red");
                        } else {
                            if (!check(search_jobType.val()) && !t.tools.CheckFormNotEmpty(search_job, "岗位小类！")) {
                                $(".job-searchForm").css("border", "1px solid red");
                            }
                            else {
                                $(".job-searchForm").css("border", "1px solid #cecece");
                                var lindex = LOAD("正在筛选招聘信息...");
                                setTimeout(function () {
                                    $.get("/zhaopin/select?{0}".Format($("form#search-form-zhaopin").serialize() + "&search_worktype=" + $("input[name='search_worktype']:checked").val()), function (data) {
                                        if (data.Ok === false) {
                                            layer.msg(data.Msg);
                                        } else {
                                            $(".zhaopin-list-wrap").html(data);
                                            Leo.BootStrap_Tab_Change(".job-searchForm", ".zhaopin-list-wrap", "#zhaopinPage", "/zhaopin/select?{0}", function () { return $("form#search-form-zhaopin").serialize() + "&search_worktype=" + $("input[name='search_worktype']:checked").val() });
                                        }
                                        CLOSE(lindex);
                                    });
                                }, t.tools.Random(400, 1000));
                            }
                        }
                    });
                },
                detail: function (par) {
                    var index;
                    $(".btn-sendcv").UnBindAndBind("click", function () {
                        var divEle = $("<div>").attr("id", "cvid").addClass("Ltac");
                        var inputEle = $("<input>").attr({ "hidden": !1, type: "file" }).addClass("Ldn");
                        inputEle.appendTo(divEle);
                        var btn_subCV = $("<a>").addClass("btn-sm btn-primary").text("发送简历").click(function () {
                            if (inputEle.val()) {
                                t.SendFile("/zhaopin/cv/{0}".Format(par.id), inputEle[0], !1, function (data) {
                                    if (data.Ok) {
                                        $(".btn-sendcv").text("简历已投递").unbind("click").removeClass("btn-sendcv");
                                        t.msg("简历投递成功，请等待用人单位联系！");
                                    } else {
                                        t.msg(data.Msg || "简历投递失败");
                                    }
                                    $("#cvid").remove();
                                    layer.close(index);
                                });
                            } else {
                                t.msgfail("请选择简历再发送！");
                            }
                        });
                        divEle.append(btn_subCV);
                        $("body").append(divEle);
                        index = AlertDiv_End("#cvid", "300px", "120px", "选择简历", null, function () {
                            $("#cvid").remove();
                        });
                        //inputEle.click();
                    });
                    t.InitLikeControl.call(t, 4, par.id);
                    if (!par.error) {
                        t.MsgTips("信息失效提醒", "您浏览的信息已过期，暂不可进行操作！", 'lt');
                    }
                },
            };
        return function (action, p) {
            return T[action](p);
        }
    },
    qiuzhi: function () {
        var t = this,
            T = {
                common: function () {
                    var tool = t.tools, check = tool.CheckFormNotEmpty, checkLength = tool.CheckLength;
                    function InitInput() {
                        Leo.tools.PickupInput("#JianLiPic", "#JianLiPic-input");
                        $("input[name='WorkType']").change(function () {
                            if ($("input[name='WorkType']:checked").val() == "2") {
                                $("#jianzhi").removeClass("Ldn");
                            } else {
                                $("#jianzhi").addClass("Ldn");
                            }
                        });
                    }

                    function Sub(addorEdit, feeType) {
                        var me = $(this), _t = T;
                        if (check("#IWant", "求职意向")) {
                            if (checkLength("#IWant".val(), 1, 20, "求职意向")) {
                                if (check("#BelongJobTrade", "行业")) {
                                    if (check("#BelongJob", "岗位小类")) {
                                        if (check("#deadTime", "过期时间")) {
                                            if (parseInt("#deadTime".val()) >= 15 && parseInt("#deadTime".val()) <= 45) {
                                                if (check("#Money", "求职薪资")) {
                                                    if (check("#IWantPlace", "期望工作地点")) {
                                                        if (checkLength("#IWantPlace".val(), 1, 15, "期望工作地点")) {
                                                            if (check("#NowWork", "目前岗位")) {
                                                                if (checkLength("#NowWork".val(), 1, 20, "目前岗位")) {
                                                                    if (check("#WorkStatus", "离职状态")) {
                                                                        if (check("#Study", "学历")) {
                                                                            if (check("#WorkYear", "工作年限")) {
                                                                                if (check("#Contact", "联系方式")) {
                                                                                    if (checkLength($("#Contact").val(), 1, 50, "联系方式")) {
                                                                                        if (check("#MyDesc", "自我简介")) {
                                                                                            if (checkLength("#MyDesc".val(), 1, 200, "自我简介")) {
                                                                                                if (check("#SelfAssessment", "自我评价")) {
                                                                                                    if (checkLength("#SelfAssessment".val(), 1, 200, "自我评价")) {
                                                                                                        //if (check("#JianLiPic-input", "简历信息")) {
                                                                                                        var layerIndex = LOAD(), desc, formdata = new FormData();
                                                                                                        me.unbind("click");
                                                                                                        if (addorEdit == 1) {
                                                                                                            desc = "发布";
                                                                                                            if (feeType == 1) {
                                                                                                                $("#nmform").attr("action", "/pay/qiuzhipay");
                                                                                                            }
                                                                                                        } else {
                                                                                                            desc = "编辑";
                                                                                                            ($("#JianLiPic").data("ischanged") && (formdata.append("JianLiPic", $("#JianLiPic")[0].files[0]), formdata.append("ischange", true))), formdata.append("QiuZhiID", $("#QiuZhiID").val()), $("#nmform").attr("action", "/qiuzhi/edit")
                                                                                                        }
                                                                                                        var hasPic = false;
                                                                                                        !t.tools.isEmptyObject("#JianLiPic-input".val()) && (hasPic = true, formdata.append("JianLiPic", $("#JianLiPic")[0].files[0]));
                                                                                                        formdata.append("hasPic", hasPic);
                                                                                                        formdata.append("IWant", $("#IWant").val());
                                                                                                        formdata.append("deadTime", $("#deadTime").val());
                                                                                                        formdata.append("Money", $("#Money").val());
                                                                                                        formdata.append("NowWork", $("#NowWork").val());
                                                                                                        formdata.append("WorkStatus", $("#WorkStatus").val());
                                                                                                        formdata.append("Study", $("#Study").val());
                                                                                                        formdata.append("WorkYear", $("#WorkYear").val());
                                                                                                        formdata.append("MyDesc", $("#MyDesc").val());
                                                                                                        formdata.append("Contact", $("#Contact").val());
                                                                                                        formdata.append("SelfAssessment", $("#SelfAssessment").val());
                                                                                                        formdata.append("IWantPlace", $("#IWantPlace").val());
                                                                                                        formdata.append("WorkType", $("input[name='WorkType']:checked").val());
                                                                                                        if ($("input[name='WorkType']:checked").val() == "2") {
                                                                                                            formdata.append("WorkTime", "{0}_{1}_{2}_{3}_{4}".Format($("#SelectworkMonth").val(), $("#SelectworkWeek").val(), $("#SelectworkStartWeek").val(), $("#SelectworkEndWeek").val(), $("#SelectworkHour").val()));
                                                                                                        }
                                                                                                        formdata.append("BelongJobTrade", $("#BelongJobTrade").val());
                                                                                                        formdata.append("BelongJob", $("#BelongJob").val());
                                                                                                        $.ajax({
                                                                                                            url: $("#nmform").attr("action"),
                                                                                                            type: "post",
                                                                                                            dataType: "json",
                                                                                                            contentType: false,
                                                                                                            processData: false,
                                                                                                            data: formdata,
                                                                                                            success: function (data) {
                                                                                                                if (data.Ok) {
                                                                                                                    feeType == 1 ? t.msgsuccess("正在跳转支付宝支付") : (layer.msg("求职信息{0}成功".Format(desc)));
                                                                                                                    location.href = data.Url;
                                                                                                                } else {
                                                                                                                    layer.msg(data.Msg || "{0}失败".Format(desc));
                                                                                                                    me.bind("click", function () {
                                                                                                                        _t.common.Submit.call(this, addorEdit, feeType);
                                                                                                                    });
                                                                                                                }
                                                                                                                layer.close(layerIndex);
                                                                                                            }
                                                                                                        });
                                                                                                        //}
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            } else {
                                                $("#deadTime").focus();
                                                t.msgfail("过期时间15~45天内");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return {
                        init: InitInput,
                        Submit: Sub
                    }
                }(),
                index: function () {
                    var check = t.tools.isEmptyObject;
                    Leo.BootStrap_Tab_Change(".job-searchForm", ".qiuzhi-list-wrap", "#qiuzhiPage", "/qiuzhi/qiuzhis");
                    t.Search(function (keyword) {
                        var layerIndex = LOAD("正在搜索...");
                        $.get("/qiuzhi/search?key={0}".Format(keyword), function (data) {
                            if (data.Ok === false) {
                                layer.msg(data.Msg);
                            } else {
                                $(".qiuzhi-list-wrap").html(data);
                                Leo.BootStrap_Tab_Change(".job-searchForm", ".qiuzhi-list-wrap", "#qiuzhiPage", "/qiuzhi/search?key={0}", keyword);
                            }
                            CLOSE(layerIndex);
                        });
                    });
                    $(".btn-search-form").UnBindAndBind("click", function () {
                        var search_jobTrade = "#search_jobTrade", search_jobType = "#search_jobType",
                            search_job = "#search_job", search_WorkYear = "#search_WorkYear", search_workPlace = "#search_workPlace", search_money = "#search_money", search_study = "#search_study";
                        if (check(search_jobTrade.val()) && check(search_jobType.val()) && check(search_job.val()) && check(search_WorkYear.val()) && check(search_workPlace.val()) && check(search_money.val()) && check(search_study.val()) && check($("input[name='search_worktype']:checked").val())) {
                            t.msgfail("请进行任意筛选再进行搜索");
                            $(".job-searchForm").css("border", "1px solid red");
                        } else {
                            if (!check(search_jobType.val()) && !t.tools.CheckFormNotEmpty(search_job, "岗位小类！")) {
                                $(".job-searchForm").css("border", "1px solid red");
                            }
                            else {
                                $(".job-searchForm").css("border", "1px solid #cecece");
                                var lindex = LOAD("正在筛选求职信息...");
                                $.get("/qiuzhi/select?{0}".Format($("form#search-form-qiuzhi").serialize() + "&search_worktype=" + $("input[name='search_worktype']:checked").val()), function (data) {
                                    if (data.Ok === false) {
                                        layer.msg(data.Msg);
                                    } else {
                                        $(".qiuzhi-list-wrap").html(data);
                                        Leo.BootStrap_Tab_Change(".job-searchForm", ".qiuzhi-list-wrap", "#qiuzhiPage", "/qiuzhi/select?{0}", function () { return $("form#search-form-qiuzhi").serialize() + "&search_worktype=" + $("input[name='search_worktype']:checked").val() });
                                    }
                                    CLOSE(lindex);
                                });
                            }
                        }
                    });
                },
                publish: function () {
                    T.common.init();
                    $("#btn_addQiuZhi").UnBindAndBind("click", function () {
                        T.common.Submit.call(this, 1, 0);
                    }).dblclick(function () { });
                    $("#btn_addQiuZhiCash").UnBindAndBind("click", function () {
                        T.common.Submit.call(this, 1, 1);
                    });
                },
                edit: function () {
                    T.common.init();
                    $("#btn_editProductCash").UnBindAndBind("click", function () {
                        T.common.Submit.call(this, 2);
                    }).dblclick(function () { });
                },
                detail: function (par) {
                    $('.btn-vidite').UnBindAndBind("click", function () {
                        $.get("/zhaopin/zhaopininfos/{0}".Format(par.id), function (data) {
                            if (data.length > 0) {
                                var divEle = $("<div>").attr("id", "selectZhaoPin");
                                var ulEle = $("<ul>").addClass("list-group Lmgb0");
                                $.each(data, function (i, n) {
                                    ulEle.append($("<li>").css("cursor", "pointer").text(n.Name).data("zid", n.I).data("name", n.Name).addClass("list-group-item").mouseover(function () {
                                        $(this).css("background-color", "#cecece")
                                    }).mouseleave(function () {
                                        $(this).css("background-color", "#fff")
                                    }).click(function () {
                                        var name = $(this).data("name");
                                        var id = $(this).data("zid");
                                        divEle.empty();
                                        var idivEle = $("<div>").attr("id", "inviteWrap");
                                        var timeEle = $("<input>").addClass("form-control pickup-time").attr("placeholder", "选择面试日期和时间");
                                        var viewplace = $("<input>").addClass("form-control").attr("placeholder", "面试地址");
                                        var subbtn = $("<a>").text("邀请面试").addClass("btn btn-primary").click(function () {
                                            if (t.tools.CheckFormNotEmpty(timeEle, "面试时间")) {
                                                if (t.tools.CheckFormNotEmpty(viewplace, "面试地址")) {
                                                    $.post("/zhaopin/iv/{0}".Format(par.id), { zid: id, time: timeEle.val(), place: viewplace.val() },
                                                        function (data) {
                                                            if (data.Ok) {
                                                                t.msgsuccess("邀请面试成功！");
                                                                $(".btn-vidite").unbind("click").text("已邀请面试").removeClass("btn-vidite");
                                                            } else {
                                                                t.msgfail("邀请面试失败！");
                                                            }
                                                            layer.closeAll();
                                                        });
                                                }
                                            }
                                        });
                                        divEle.append($("<div>").addClass("form-group").append(timeEle), $("<div>").addClass("form-group").append(viewplace), $("<div>").addClass("form-group Ltac").append(subbtn));

                                        lay('.pickup-time').each(function () {
                                            laydate.render({
                                                elem: this,
                                                trigger: 'click',
                                                type: 'datetime',
                                                min: "{0}-{1}-{2}".Format(new Date().getFullYear(), new Date().getMonth() + 1, (new Date().getDate() + 1)),
                                            });
                                        });
                                    }));
                                });
                                $("body").append(divEle.append(ulEle));
                                AlertDiv_End("#selectZhaoPin", "350px", "200px", "选择招聘(点击选择)", null, function () {
                                    $("#selectZhaoPin").remove();
                                });
                            } else {
                                t.msgfail("您没有发布任何职位信息！无法邀请面试！");
                            }
                        });
                    });
                    t.InitLikeControl.call(t, 5, par.id);
                    if (!par.error) {
                        t.MsgTips("信息失效提醒", "您浏览的信息已过期，暂不可进行操作！", 'lt');
                    }
                },
            };
        return function (action, p) {
            return T[action](p);
        }
    },
    product: function () {
        var t = this,
            T = {
                common: function () {
                    var tool = t.tools, check = tool.CheckFormNotEmpty, checkLength = tool.CheckLength;
                    function InitInput() {
                        Leo.tools.PickupInput("#ProductPic", "#ProductPic-input");
                    }
                    function Sub(addorEdit, feeType) {
                        var me = $(this), _t = T;
                        if (check("#CompanyName", "公司名称")) {
                            if (check("#PTitle", "产品标题")) {
                                if (checkLength("#PTitle".val(), 1, 20, "产品标题")) {
                                    if (check("#PLocation", "产品定位")) {
                                        if (checkLength("#PLocation".val(), 1, 20, "产品定位")) {
                                            if (check("#PDesc", "产品介绍")) {
                                                if (checkLength("#PDesc".val(), 1, 200, "产品介绍")) {
                                                    if (check("#PFunction", "产品功能")) {
                                                        if (checkLength("#PFunction".val(), 1, 200, "产品功能")) {
                                                            if (check("#pickupUnit", "产品单位")) {
                                                                if (check("#SendDay", "发货时间")) {
                                                                    if (check("#PPrice", "产品价格")) {
                                                                        if (check("#PSize", "产品尺寸")) {
                                                                            if (checkLength("#PSize".val(), 1, 20, "产品尺寸")) {
                                                                                if (check("#PWeight", "产品重量")) {
                                                                                    if (checkLength("#PWeight".val(), 1, 20, "产品重量")) {
                                                                                        if (check("#Contact", "联系方式")) {
                                                                                            if (check("#ProductPic-input", "产品图片")) {
                                                                                                var layerIndex = LOAD(), desc, formdata = new FormData();
                                                                                                if (addorEdit == 1) {
                                                                                                    desc = "发布";
                                                                                                    if (feeType == 1) { $("#nmform").attr("action", "/pay/productpay"); }
                                                                                                } else {
                                                                                                    desc = "编辑";
                                                                                                    $("#nmform").attr("action", "/product/edit");
                                                                                                    $("#ProductPic").data("ischanged") && (formdata.append("ProductPic", $("#ProductPic")[0].files[0]), formdata.append("ischange", true));
                                                                                                    formdata.append("ProductID", $("#ProductID").val());
                                                                                                }
                                                                                                formdata.append("ProductPic", $("#ProductPic")[0].files[0]);
                                                                                                formdata.append("Contact", $("#Contact").val());
                                                                                                formdata.append("PSize", $("#PSize").val());
                                                                                                formdata.append("PPrice", $("#PPrice").val());
                                                                                                formdata.append("PFunction", $("#PFunction").val());
                                                                                                formdata.append("PDesc", $("#PDesc").val());
                                                                                                formdata.append("PLocation", $("#PLocation").val());
                                                                                                formdata.append("PTitle", $("#PTitle").val());
                                                                                                formdata.append("CompanyName", $("#CompanyName").val());
                                                                                                formdata.append("SendDay", $("#SendDay").val());
                                                                                                formdata.append("PWeight", $("#PWeight").val());
                                                                                                formdata.append("PUnit", $("#pickupUnit").val());

                                                                                                $.ajax({
                                                                                                    url: $("#nmform").attr("action"),
                                                                                                    type: "post",
                                                                                                    dataType: "json",
                                                                                                    contentType: false,
                                                                                                    processData: false,
                                                                                                    data: formdata,
                                                                                                    success: function (data) {
                                                                                                        if (data.Ok) {
                                                                                                            feeType == 1 ? t.msgsuccess("正在跳转支付宝支付") : (layer.msg("产品服务信息{0}成功".Format(desc)));
                                                                                                            location.href = data.Url;
                                                                                                        } else {
                                                                                                            layer.msg(data.Msg || "{0}失败".Format(desc));
                                                                                                            me.bind("click", function () {
                                                                                                                _t.common.Submit.call(this, addorEdit, feeType);
                                                                                                            });
                                                                                                        }
                                                                                                        layer.close(layerIndex);
                                                                                                    }
                                                                                                });
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return {
                        init: InitInput,
                        Submit: Sub
                    }
                }(),
                index: function () {
                    var check = t.tools.isEmptyObject;
                    Leo.BootStrap_Tab_Change(".product-searchForm", ".product-list-wrap", "#productPage", "/product/products");
                    t.Search(function (keyword) {
                        var layerIndex = LOAD("正在搜索...");
                        $.get("/product/search", {
                            key: keyword
                        }, function (data) {
                            if (data.Ok === false) {
                                layer.msg(data.Msg);
                            } else {
                                $(".product-list-wrap").html(data);
                            }
                            CLOSE(layerIndex);
                        });
                    });
                    $(".btn-search-form").UnBindAndBind("click", function () {
                        var search_cname = "#search_cname", search_pname = "#search_pname",
                            search_price = "#search_price", search_productEndTime = "#search_productEndTime";
                        if (check(search_cname.val()) && check(search_pname.val()) && check(search_price.val()) && check(search_productEndTime.val())) {
                            t.msgfail("请进行任意筛选再进行搜索");
                            $(".product-searchForm").css("border", "1px solid red");
                        } else {
                            $(".product-searchForm").css("border", "1px solid #cecece");
                            var lindex = LOAD("正在筛选产品信息...");
                            setTimeout(function () {
                                $.get("/product/select?{0}".Format($("form#search-form-product").serialize()), function (data) {
                                    if (data.Ok === false) {
                                        layer.msg(data.Msg);
                                    } else {
                                        $(".product-list-wrap").html(data);
                                        Leo.BootStrap_Tab_Change(".product-searchForm", ".product-list-wrap", "#productPage", "/product/select?{0}", function () { return $("form#search-form-product").serialize() });
                                    }
                                    CLOSE(lindex);
                                });
                            }, t.tools.Random(400, 1000));
                        }
                    });
                    lay('.pickup-time').each(function () {
                        laydate.render({
                            elem: this,
                            trigger: 'click',
                            type: 'datetime',
                            min: "{0}-{1}-{2}".Format(new Date().getFullYear(), new Date().getMonth() + 1, (new Date().getDate() + 1)),
                        });
                    });
                },
                detail: function (par) {
                    t.ShowMaxImg();
                    $(".btn-orderbuy").UnBindAndBind("click", function () {
                        AlertConfirm("确定要预约吗？确定将发送短信给商家！", "确定预约购买", "稍后", function () {
                            $.post("/product/orderbuy/{0}".Format(par.id), function (data) {
                                if (data.Ok) {
                                    t.msgsuccess("预约成功");
                                } else {
                                    t.msgfail(data.Msg || "预约失败");
                                }
                            });
                        })
                    });
                    if (!par.error) {
                        t.MsgTips("信息失效提醒", "您浏览的信息已过期，暂不可进行操作！", 'lt');
                    }
                },
                publish: function () {
                    T.common.init();
                    $("#btn_addProduct").UnBindAndBind("click", function () {
                        T.common.Submit(1, 0);
                    });
                    $("#btn_addProductCash").UnBindAndBind("click", function () {
                        T.common.Submit(1, 1);
                    });
                },
                edit: function (p) {
                    T.common.init();
                    $("#btn_addProductCash").UnBindAndBind("click", function () {
                        T.common.Submit(2);
                    });
                }
            };
        return function (action, p) {
            return T[action](p);
        }
    },
    bbs: function () {
        var t = this, check = t.tools.CheckFormNotEmpty;
        var T = {
            autoSaveIntervalIndex: -1,
            autoSaveItemId: 0,//ID
            cacheKey: function () { return this.autoSaveItemId === 0 ? "BBS_ADD_" + t.user.id : "BBS_EDIT_{0}_{1}".Format(this.autoSaveItemId, t.user.id) },
            /**自动保存*/
            autoSave: function (itemId) {
                var thisarea = this;
                var noEmpty = t.tools.isNotEmptyObject;
                thisarea.autoSaveItemId = itemId || 0;
                thisarea.renderOldContent();
                thisarea.autoSaveIntervalIndex = setInterval(function () {
                    //判断有改变就保存
                    var obj = {
                        isanonymous: thisarea.isAnonymous(),//是否匿名发布
                        title: $("#qtitle").val(),//标题
                        menuType: $("#menuType").val(),//帖子类别
                        tags: thisarea.addTagsForPost(true),//标签
                        body: encodeURI($("#summernote").summernote("code")),//内容
                        contentNeedFee: $("#questionContentCoinStatus").is(":checked"),//内容付费
                        contentFeeType: $("#bbscontentFeeType").val(),//内容付费类型
                        contentFee: $("#questionContentFee").val(),//内容付费量
                        questionCoin: $("#questionCoin").is(":checked"),//答题悬赏
                        coin: $("#payCoin").val(),//悬赏值
                    }
                    if (obj.isanonymous || noEmpty(obj.title) || noEmpty(obj.menuType)
                        || obj.tags.length > 0 || !$("#summernote").summernote("isEmpty") || obj.contentNeedFee
                        || noEmpty(obj.contentFeeType) || noEmpty(obj.contentFee)
                        || obj.questionCoin || noEmpty(obj.coin)) {
                        localStorage.setItem(thisarea.cacheKey(), JSON.stringify(obj));
                    } else {
                        localStorage.removeItem(thisarea.cacheKey());
                    }
                }, 1000);
            },
            /**停止自动保存，并删除已保存的数据 */
            removeAutoSave: function () {
                clearInterval(this.autoSaveIntervalIndex);
                localStorage.removeItem(this.cacheKey());
                this.autoSaveIntervalIndex = -1
                this.autoSaveItemId = 0;
            },
            /**加载旧内容 */
            renderOldContent: function () {
                var content = localStorage.getItem(this.cacheKey())
                if (!t.tools.isEmptyObject(content)) {
                    t.msg("检测到有您上次编辑未发布内容，已自动还原", null, null, null, 3000);
                    content = JSON.parse(content);
                    $("#add_anonymous").attr("checked", content.isanonymous);//是否匿名发布
                    $("#qtitle").val(content.title)//标题
                    $("#menuType").val(content.menuType)//帖子类别
                    //tags: thisarea.addTagsForPost(true),//标签
                    var taglistwrap = $(".TagListWrap");
                    if (this.autoSaveItemId != 0) {
                        taglistwrap.children().remove();
                    }
                    $.each(content.tags, function (i, tag) {
                        taglistwrap.append($("<span>").addClass("label label-success Lmgr5").attr({ "data-toggle": "tooltip", "title": "点击移除此标签！" }).text(tag[0]).data("value", tag)
                            .click(function () {
                                //关闭当前添加的标签
                                $(this).remove();
                                taglistwrap.find(".tooltip").remove();
                                taglistwrap.data("TagCount", taglistwrap.children().length);
                                $("#btn-addtag").removeClass("Ldn");
                            }));
                    });
                    if (content.tags.length == 0) {
                        taglistwrap.next().removeClass("Ldn");
                    }
                    $("#summernote").summernote("code", decodeURI(content.body))//内容
                    if (content.contentNeedFee) {
                        $("#questionContentCoinStatus").attr("checked", content.contentNeedFee)//内容付费
                        $("#bbscontentFeeType").val(content.contentFeeType).attr("disabled", false)//内容付费类型
                        $("#questionContentFee").val(content.contentFee).attr("disabled", false)//内容付费量
                    } else {
                        $("#bbscontentFeeType").val(content.contentFeeType)//内容付费类型
                        $("#questionContentFee").val(content.contentFee)//内容付费量
                    }
                    if (content.questionCoin) {
                        $("#questionCoin").attr("checked", content.questionCoin)//答题悬赏
                        $("#payCoin").val(content.coin).attr("disabled", false)//悬赏值
                    } else {
                        $("#payCoin").val(content.coin)//悬赏值
                    }
                    t.ToolTip();
                }
            },
            //举报
            report: function () {
                $(".report").UnBindAndBind("click", function () {
                    AlertPromptTextarea("举报", null, "举报", null, function () {
                        var type = me.data("type"), id = me.data("mid");
                        var desc = textarea.val();
                        if (desc) {
                            var lindex = LOAD("举报中")
                            $.post("/common/report", { type: type, id: id, desc: desc }, function (data) {
                                if (data.Ok) {
                                    t.msgsuccess("举报成功，等待管理员核实");
                                } else {
                                    t.msgfail(data.Msg || "举报失败");
                                }
                                layer.close(lindex);
                            });
                        } else {
                            t.msgfail("请填写举报内容");
                        }
                    }, 100);
                    Leo.msg("请输入举报内容");
                });
            },
            //查看反对人数
            showAgainstUser: function () {
                $(".showPriseUser").UnBindAndBind("click", function () {
                    var me = $(this);
                    var type = me.data("type");
                    var id = me.data("mid");
                    $.get("/common/getPriseUsers", { type: type, mid: id }, function (data) {
                        if (data.Ok && data.Data.length > 0) {
                            var div = $("<div>").addClass("Ldn");
                            var table = $("<table>").addClass("table table-hover table-bordered Ltac");
                            table.append($("<tr>").append(
                                $("<th>").addClass("Ltac").text("用户"),
                                $("<th>").addClass("Ltac").text("操作时间")
                            ));
                            $.each(data.Data, function () {
                                table.append($("<tr>").append(
                                    $("<td>").append($("<a>").attr({ "href": "/user/detail/" + this.id, "target": "_blank" }).css("color", "#24b654").text(this.name)),
                                    $("<td>").text(t.tools.ChangeDateFormat(this.time))
                                ));
                            });
                            $("body").append(div.append(table));
                            AlertDiv(div, "330px", "300px", "点赞人数", null);
                            //AlertActionAreaWithConfirmWithSize(div, "操作用户", "300px", null, "确定", null, null, null);
                        } else {
                            t.msgfail("无数据");
                        }
                    });
                });
                $(".showAgainstUser").UnBindAndBind("click", function () {
                    var me = $(this);
                    var type = me.data("type");
                    var id = me.data("mid");
                    $.get("/common/getAgainstUsers", { type: type, mid: id }, function (data) {
                        if (data.Ok && data.Data.length > 0) {
                            var div = $("<div>").addClass("Ldn");
                            var table = $("<table>").addClass("table table-hover table-bordered Ltac");
                            table.append($("<tr>").append(
                                $("<th>").addClass("Ltac").text("用户"),
                                $("<th>").addClass("Ltac").text("操作时间")
                            ));
                            $.each(data.Data, function () {
                                table.append($("<tr>").append(
                                    $("<td>").append($("<a>").attr({ "href": "/user/detail/" + this.id, "target": "_blank" }).css("color", "#24b654").text(this.name)),
                                    $("<td>").text(t.tools.ChangeDateFormat(this.time))
                                ));
                            });
                            $("body").append(div.append(table));
                            AlertDiv(div, "330px", "300px", "反对人数", null);
                        } else {
                            t.msgfail("无数据");
                        }
                    });
                });
            },
            //是否匿名发布
            isAnonymous: function () {
                return $("#add_anonymous").is(":checked");
            },
            attach: function (p) {
                var attachCount = p ? p[0] : 0, isedit = p ? p[1] : false;
                var attachmentIndex = (attachCount || 0) + 1,//附件序号初始为1
                    attachArr = {};//附件存放数组

                //循环初始化已存在的附件的表单
                function ForeachInitAttachForm() {
                    if (attachCount && attachCount > 0) {
                        for (var i = 1; i <= attachCount; i++) {
                            var fileShowArea = "#fileShowArea_" + i,
                                addfile = "#addFile_" + i;
                            ListenPickupChange(addfile, fileShowArea, i);
                            deleteFile(i);
                            FeeTypeChange(i);
                            ListenFeeChange(i);
                            attachArr[i] || (attachArr[i] = {});
                            attachArr[i].element = "#addFile_" + i;
                            attachArr[i].attachId = $("#attachMid_" + i).val();
                        }
                    }
                }

                //附件相关
                function deleteFile(attachIndex) {
                    var removefile = "#btn-removeFile_" + attachIndex,
                        attachmentwrapper = ".attachmentwrapper_" + attachIndex;

                    $(removefile).removeClass("Ldn").UnBindAndBind("click", function () {
                        $(".attachmentaction_" + attachIndex).remove();
                        $(attachmentwrapper).remove();
                        delete attachArr[attachIndex];
                    });
                }

                //初始化
                function InitAddAttach(attachIndex) {
                    var btn_addfile = "#btn-addFile_" + attachIndex,
                        fileShowArea = "#fileShowArea_" + attachIndex,
                        addfile = "#addFile_" + attachIndex,
                        removefile = "#btn-removeFile_" + attachIndex,
                        attachmentwrapper = ".attachmentwrapper_" + attachIndex;

                    $(btn_addfile).UnBindAndBind("click", function () {
                        //显示 
                        $(btn_addfile).hide();
                        $(attachmentwrapper).removeClass("Ldn");
                        $(addfile).click();
                        deleteFile(attachIndex);
                        ListenPickupChange(addfile, fileShowArea, attachIndex, true);
                    });
                }

                function ListenPickupChange(addfile, fileShowArea, attachIndex, isnew) {
                    t.tools.PickupInput(addfile, fileShowArea, function () {
                        if ($(this).val()) {
                            //添加了附件的话，加到数组里

                            attachArr[attachIndex] || (attachArr[attachIndex] = {});
                            attachArr[attachIndex].element = addfile;
                            attachArr[attachIndex].ischange = isedit ? true : false;
                            attachArr[attachIndex].isnew = isnew || false;
                            if (attachIndex == attachmentIndex) {
                                FeeTypeChange(attachIndex);
                                //生成下一下
                                attachmentIndex++;
                                AddNextFileHtml(attachmentIndex);
                                InitAddAttach(attachmentIndex);
                            }
                        } else {
                            $(btn_addfile).show();
                            $(removefile).hide();
                        }
                    });
                }

                function ListenFeeChange(attachIndex) {
                    //FileFee_1
                    $("#FileFee_" + attachIndex).on("change input", function (e) {
                        if ($(this).val()) {
                            attachArr[attachIndex]["feechange"] = true;
                        }
                    });
                }

                //添加下一个
                function AddNextFileHtml(attachIndex) {
                    var html = [];
                    html.push('<div class="attachmentwrapper_{0} col-md-8 Lpdl0 Lmgt5 Ldn">'.Format(attachIndex));
                    html.push('<input hidden="hidden" style="display:none !important;" type="file" id="addFile_{0}" />'.Format(attachIndex));
                    html.push('<div class="col-md-4 Lpdl0i">');
                    html.push('<input type="text" id="fileShowArea_{0}" class="form-control" placeholder="添加附件" />'.Format(attachIndex));
                    html.push('</div>');
                    html.push('<div class="col-md-4">');
                    html.push('<select id="filefeetype_{0}" class="form-control">'.Format(attachIndex));
                    html.push('<option value="">费用类型</option>');
                    html.push('<option value="0">免费</option>');
                    html.push('<option value="10">积分支付</option>');
                    html.push('<option value="20">VIP分支付</option>');
                    html.push('</select>');
                    html.push('</div>');
                    html.push('<div class="col-md-4">');
                    html.push('<input class="form-control" id="FileFee_{0}" min="0" name="" placeholder="附件费用" type="number" value="">'.Format(attachIndex));
                    html.push('</div>');
                    html.push('</div>');
                    html.push('<div class="attachmentaction_{0} col-md-3 Lpdl0i Lmgt5">'.Format(attachIndex));
                    html.push('<a id="btn-removeFile_{0}" class="btn btn-warning Ldn">-删除附件</a>'.Format(attachIndex));
                    html.push('<a id="btn-addFile_{0}" class="btn btn-warning">+添加附件(可选)</a>'.Format(attachIndex));
                    html.push('</div>');
                    $(".attachmentwrappers").append(html.join(''));
                }

                //更改费用类型时触发
                function FeeTypeChange(attachindex) {
                    var feetype = "#filefeetype_" + attachindex;
                    $(feetype).change(function () {
                        var v = $(this).val();
                        if (v == "0") {
                            $("#FileFee_" + attachindex).attr("disabled", !0).val("0");
                        } else {
                            $("#FileFee_" + attachindex).attr("disabled", !1).val("");
                        }
                        attachArr[attachindex].feetypechange = true;
                    });
                }

                //校验
                function validateAttach() {
                    var obj = {
                        fees: []
                    }
                    var keys = Object.keys(attachArr);
                    var ok = true;
                    for (var i in keys) {
                        var index = keys[i];
                        var feetype = "#filefeetype_" + index;
                        var filefee = "#FileFee_" + index;
                        var _feeobj = {};
                        if (check($(feetype), "费用类型")) {
                            _feeobj = {
                                index: index,
                                feeType: $(feetype).val(),
                                isChange: attachArr[index].ischange,
                                feetypechange: attachArr[index].feetypechange,
                                feechange: attachArr[index].feechange,
                                attachId: attachArr[index].attachId,
                                isnew: attachArr[index].isnew
                            }
                            if (check($(filefee), "附件费用")) {
                                _feeobj["fee"] = $(filefee).val();
                            } else {
                                ok = false;
                                break;
                            }
                            obj.fees.push(JSON.stringify(_feeobj));
                        } else {
                            FeeTypeChange(index);
                            ok = false;
                            break;
                        }
                    }
                    obj.ok = ok;
                    return obj;
                }

                function getIndex() {
                    return attachmentIndex;
                }
                function getAttachArr() {
                    return attachArr;
                }

                ForeachInitAttachForm();
                InitAddAttach(attachmentIndex);

                return {
                    validateAttach: validateAttach,
                    FeeTypeChange: FeeTypeChange,
                    AddNextFileHtml: AddNextFileHtml,
                    InitAddAttach: InitAddAttach,
                    deleteFile: deleteFile,

                    getIndex: getIndex,
                    getAttachArr: getAttachArr
                }
            },
            initAddTag: function () {
                $("#btn-addtag").UnBindAndBind("click", function () {
                    var topic = $("#menuType");
                    if (check(topic, "请选择发布板块", !1)) {
                        AlertDiv_End("#addTagWrapper", "450px", "350px", "添加标签", null, function () {
                            $("#inputTag,#newTagInput").val("");
                        });
                    }
                });
                $("#menuType").change(function () {
                    $(".TagListWrap").empty();
                    $("#btn-addtag").removeClass("Ldn");
                });
            },
            addTagsForPost: function (isGetDetail) {
                var tags = [], tagWrap = $(".TagListWrap").children("span");
                if (tagWrap.length > 0) {
                    $.each(tagWrap, function (i, n) {
                        var tag = $(this);
                        if (isGetDetail) {
                            tags.push(tag.data("value"));
                        } else {
                            tags.push(tag.data("value")[1]);
                        }
                    });
                }
                return tags;
                //return tags.length == 0 ? null : tags;
            },
            add: function () {
                var thisarea = this;

                thisarea.initAddTag();

                var attachFun = thisarea.attach();

                $("#questionCoin").change(function () {
                    $("#payCoin").attr("disabled", this.checked ? !1 : !0).val("");
                });

                //内容付费
                $("#questionContentCoinStatus").change(function () {
                    if (this.checked) {
                        $("#bbscontentFeeType").attr("disabled", false);
                        $("#questionContentFee").attr("disabled", false);
                    } else {
                        $("#bbscontentFeeType").attr("disabled", true);
                        $("#questionContentFee").attr("disabled", true);
                    }
                });

                $("#btn_ask").unbind("click").click(function () {
                    if (check($("#qtitle"), "标题")) {
                        if (check($("#menuType"), "请选择板块", !0)) {
                            if (!$("#summernote").summernote("isEmpty")) {
                                if ($("#questionCoin").is(":checked")) {
                                    if (!check($("#payCoin"), "请选择悬赏类型！")) {
                                        return !1;
                                    }
                                }

                                //管理付费内容
                                if ($("#questionContentCoinStatus").is(":checked")) {
                                    if (!check($("#bbscontentFeeType"), "内容付费类型")) {
                                        return !1
                                    } else if (!check($("#questionContentFee"), "内容费用")) {
                                        return !1;
                                    }
                                }
                                //管理附件
                                //var fujian = $("#addFile");
                                var validate = attachFun.validateAttach();
                                if (validate.ok) {
                                    var _title = $("#qtitle").val();
                                    var formdata = new FormData();
                                    formdata.append("title", _title);
                                    formdata.append("body", encodeURI($("#summernote").summernote("code")));
                                    formdata.append("questionCoin", $("#questionCoin").is(":checked"));
                                    formdata.append("coin", $("#payCoin").val());
                                    formdata.append("menuType", $("#menuType").val());
                                    formdata.append("tags", thisarea.addTagsForPost());

                                    //添加内容付费
                                    formdata.append("contentNeedFee", $("#questionContentCoinStatus").is(":checked"));
                                    formdata.append("contentFeeType", $("#bbscontentFeeType").val());
                                    formdata.append("contentFee", $("#questionContentFee").val());

                                    //添加 是否 匿名发布
                                    formdata.append("isanonymous", thisarea.isAnonymous());

                                    var layerIndex = layer.load("正在发布{0}".Format(_title));
                                    //包装附件
                                    var form_attachIndexs = [];
                                    // t.tools.isEmptyObject(fujian.val()) ? null : $("#addFile")[0]
                                    var attkeys, attachArr = attachFun.getAttachArr();
                                    if (attkeys = Object.keys(attachArr), attkeys.length > 0) {
                                        for (var i in attkeys) {
                                            form_attachIndexs.push(attkeys[i]);
                                            var fujian = $(attachArr[attkeys[i]]["element"]);
                                            if (!t.tools.isEmptyObject(fujian.val())) {
                                                formdata.append("file" + attkeys[i], fujian[0].files[0]);
                                            }
                                        }
                                    }
                                    if (form_attachIndexs.length > 0) {
                                        formdata.append("attachIndexs", form_attachIndexs.join(","));
                                        formdata.append("fees", validate.fees.join(","))
                                    }

                                    t.SendFile2("/bbs/Add?t={0}".Format(t.getPK()), !1, function (data) {
                                        if (data.Ok) {
                                            $("body").empty();
                                            layer.msg(data.Msg || "发布成功");
                                            layer.close(layerIndex);
                                            thisarea.removeAutoSave();
                                            setTimeout(function () {
                                                layer.load("正在跳转到：{0}".Format(_title));
                                                location.href = data.Url;
                                            }, 500);
                                        } else {
                                            if (data.ID != undefined) {
                                                layer.msg(data.Msg);
                                            } else {
                                                location.href = "/account/login";
                                            }
                                            layer.close(layerIndex);
                                        }
                                    }, formdata, null, null);
                                }
                            } else {
                                t.msg("内容主体不能为空");
                                $('#summernote').summernote("focus");
                            }
                        }
                    }
                });
                t.RichText("#summernote", "/tool/uploadimg", "此处填写发贴内容", 1000, 0, function (data, onImageRenderCallBack) {
                    //if (imgIndex == undefined || imgIndex == null || imgIndex == 0) {
                    //    $(".note-placeholder").css("display", "none");
                    //    $(".note-editable").val("");
                    //}
                    //$(".note-editable").append($("<img>").attr("src", data.Url));
                    $("#summernote").summernote('insertImage', data.Url, function () {
                        onImageRenderCallBack && onImageRenderCallBack();
                    });
                    //onBatchUploadImageCallBack && onBatchUploadImageCallBack();
                });
                $("#qtitle").focus();
                thisarea.autoSave();
            },
            edit: function (p) {
                var thisarea = this;
                thisarea.initAddTag();
                var check = t.tools.CheckFormNotEmpty;

                //获取有几个已有附件
                var attachCount = $(".attachmentwrapper").length - 1;
                var attachFun = thisarea.attach([attachCount, true]);
                ////附件相关
                //function deleteFile(show) {
                //    var deleteEle = $("#btn-removeFile");
                //    show && (deleteEle.removeClass("Ldn"));
                //    deleteEle.UnBindAndBind("click", function () {
                //        $(this).addClass("Ldn");
                //        $("#btn-addFile").show();
                //        var showarea = $("#fileShowArea");
                //        $("#addFile").val("");
                //        $("#addFile").data("ischanged", !1).data("isdelete", !0);
                //        showarea.val(""), showarea.hide(), showarea.parent().addClass("Ldn");
                //    });
                //}
                //deleteFile(!1);

                //内容付费
                $("#questionContentCoinStatus").change(function () {
                    if (this.checked) {
                        $("#bbscontentFeeType").attr("disabled", false);
                        $("#questionContentFee").attr("disabled", false);
                    } else {
                        $("#bbscontentFeeType").attr("disabled", true);
                        $("#questionContentFee").attr("disabled", true);
                    }
                });

                //$("#btn-addFile").UnBindAndBind("click", function () {
                //    $("#addFile").click();
                //    $("#fileShowArea").parent().removeClass("Ldn");
                //    t.tools.PickupInput("#addFile", "#fileShowArea", function () {
                //        if ($(this).val()) {
                //            $("#btn-addFile").hide();
                //            deleteFile(1);
                //        } else {
                //            $("#btn-addFile").show();
                //            $("#btn-removeFile").hide();
                //        }
                //    });
                //});

                //提交编辑
                $(".btn-edit").unbind("click").bind("click", function () {
                    if (check("#qtitle", "标题")) {
                        if (!$("#summernote").summernote("isEmpty")) {
                            if (check("#menuType", "请选择问题板块", !1)) {
                                //管理付费内容
                                if ($("#questionContentCoinStatus").is(":checked")) {
                                    if (!check($("#bbscontentFeeType"), "内容付费类型")) {
                                        return !1
                                    } else if (!check($("#questionContentFee"), "内容费用")) {
                                        return !1;
                                    }
                                }

                                //管理附件
                                //var fujian = $("#addFile");
                                var validate = attachFun.validateAttach();
                                if (validate.ok) {
                                    var uriID = location.pathname.split("/")[3];
                                    var formdata = new FormData();
                                    formdata.append("id", p == uriID ? p : uriID);
                                    formdata.append("title", $("#qtitle").val());
                                    formdata.append("body", encodeURI($("#summernote").summernote("code")));
                                    formdata.append("menuType", $("#menuType").val());
                                    formdata.append("tags", thisarea.addTagsForPost());

                                    //添加 是否 匿名发布
                                    formdata.append("isanonymous", thisarea.isAnonymous());

                                    //添加内容付费
                                    formdata.append("contentNeedFee", $("#questionContentCoinStatus").is(":checked"));
                                    formdata.append("contentFeeType", $("#bbscontentFeeType").val());
                                    formdata.append("contentFee", $("#questionContentFee").val());

                                    //包装附件
                                    var form_attachIndexs = [];
                                    // t.tools.isEmptyObject(fujian.val()) ? null : $("#addFile")[0]
                                    var attkeys, attachArr = attachFun.getAttachArr();
                                    if (attkeys = Object.keys(attachArr), attkeys.length > 0) {
                                        for (var i in attkeys) {
                                            form_attachIndexs.push(attkeys[i]);
                                            var fujian = $(attachArr[attkeys[i]]["element"]);
                                            if (!t.tools.isEmptyObject(fujian.val())) {
                                                formdata.append("file" + attkeys[i], fujian[0].files[0]);
                                            }
                                        }
                                    }
                                    if (form_attachIndexs.length > 0) {
                                        formdata.append("attachIndexs", form_attachIndexs.join(","));
                                        formdata.append("fees", validate.fees.join(","))
                                    }

                                    //var _ischanged = $("#addFile").data("ischanged");
                                    //if (_ischanged) {
                                    //    formdata.append("ischanged", !0);
                                    //}
                                    //formdata.append("isdelete", $("#addFile").data("isdelete"));
                                    t.SendFile2("/bbs/Edit?t={0}".Format(t.getPK()), !1, function (data) {
                                        if (data.Ok) {
                                            t.msg("正在跳转到明细页面...");
                                            thisarea.removeAutoSave();
                                            location.href = data.Url;
                                        } else {
                                            if (data.Code) {
                                                layer.msg(data.Msg);
                                            } else {
                                                location.href = "/account/login";
                                            }
                                        }
                                    }, formdata, null, 2);
                                }
                            }
                        } else {
                            layer.msg("内容主体不能为空"); return !1;
                        }
                    }
                });
                thisarea.autoSave(p);
            },
            topic: function (topic) {
                t.Search(function (keyword) {
                    var layerIndex = LOAD("正在搜索...");
                    $.get("/bbs/Search", { key: keyword, topic: location.pathname.split("/")[3] }, function (data) {
                        if (data.Ok === false) {
                            layer.msg(data.Msg);
                        } else {
                            $(".aw-explore-list .mod-body .aw-common-list").html(data);
                        }
                        CLOSE(layerIndex);
                    });
                });
                t.BootStrap_Tab_Change(".aw-explore-list", ".aw-common-list", "#bbs_page_wrap", "/bbs/PageB/{0}".Format(topic));
                $(".btn-buyOtherItem").UnBindAndBind("click", function () {
                    if (t.user.login) {
                        var me = $(this);
                        AlertConfirm("确定要花{0}积分查看吗？".Format(me.data("c")), '确定', '取消', function () {
                            $.post("/bbs/feeseeother/{0}".Format(me.data("id")), function (res) {
                                if (res.Ok) {
                                    t.msgsuccess(res.Msg || "付费成功");
                                    location.href = res.Data
                                } else {
                                    t.msgfail(res.Msg || "失败");
                                }
                            });
                        });
                    } else {
                        t.gologin();
                    }
                });
            },
            detail: function (p) {
                var thisArea = this;
                $(".FeeScoreForAnswer").click(function () {
                    var $this = $(this),
                        _id = $this.data("id"),
                        _type = $this.data("type"),
                        _coin = $this.data("coin"),
                        _layerInde = AlertConfirm("确认使用{0}{1}查看该答案吗?".Format(_coin, _type == 1 ? "积分" : "金钱"), "购买", "我再想想",
                            function () {
                                var layerMask = layer.load("正在购买，请等待…");
                                $.post("/bbs/feeanswer/{1}?t={0}".Format(Leo.getPK(), _id), function (data) {
                                    if (data.Ok) {
                                        layer.msg(data.Msg);
                                        //setTimeout(function () {
                                        //    layer.close(_layerInde);
                                        //    layer.close(layerMask);
                                        //    location.reload();
                                        //}, 500)
                                        layer.close(_layerInde);
                                        layer.close(layerMask);
                                        $(".commentContent_" + _id).html(data.Data);
                                        t.TableRemoveStyle();
                                    } else {
                                        layer.msg(data.Msg, function () { });
                                    }
                                });
                            }, function () { });
                });
                $(".btn_best").unbind("click").click(function () {
                    var me = $(this),
                        id = me.data("comment-id"),
                        qid = me.data("mid");
                    $(".btn_best").remove();
                    $.post("/bbs/Bset/{0}?t=".Format(id) + t.getPK(), {
                        qid: qid
                    }, function (data) {
                        if (data.Ok) {
                            MSG(data.Msg || "采纳成功", function () { location.reload(!0); });
                        } else {
                            layer.msg(data.Msg || "失败", function () {
                                location.reload(!0);
                            })
                        }
                    })
                });

                //附件下载
                $(".buy-attach-btn").click(function () {
                    var me = $(this),
                        fee = me.data("fee"),
                        feetype = me.data("feetype"),
                        aid = me.data("aid"),
                        filename = me.data("filename"),
                        mid = me.data("mid");
                    var confirmIndex = AlertConfirm("确定要花{0}{1}购买 {2} 吗？如若购买请继续！".Format(fee, feetype == 10 ? '积分' : "VIP分", filename), '购买并下载', '再想想', function () {
                        layer.close(confirmIndex);
                        var layerindex = LOAD("正在购买并下载");
                        //判断金钱是否足额
                        $.get("/user/enoughmoney", { fee: fee, feetype: feetype }, function (res) {
                            if (res.Ok) {
                                layer.close(layerindex);
                                if (res.Data) {
                                    var url = t.baseUrl + "/down?mt={2}&md={0}&ad={1}".Format(mid, aid, 1);
                                    $(".hasbuyerd_" + aid).hide();
                                    $(".downHref_" + aid).attr({
                                        "href": url,
                                        "target": "_blank"
                                    })
                                    window.open(url, "_blank");
                                } else {
                                    t.msgfail("对不起，您的余额不足！");
                                }
                            }
                        })
                    });
                });

                //购买主题
                $(".buycontent").click(function () {
                    var me = $(this),
                        feetype = me.data("feetype"),
                        fee = me.data("fee"),
                        mid = me.data("mid");
                    var lindex = AlertConfirm("确定要花{0}{1}购买主题内容吗？".Format(fee, feetype == 10 ? "积分" : "VIP分"), "购买", "取消", function () {
                        layer.close(lindex);
                        lindex = LOAD("购买中");
                        $.post("/buy", {
                            fee: fee, feetype: feetype, mid: mid, maintype: 1
                        }, function (res) {
                            if (res.Ok) {
                                t.msgsuccess(res.Msg);
                                $(".contentLocked").addClass("Ldn").after(
                                    $("<table>").append($("<tr>").append($("<td>").addClass("t_f").html(res.Data)))
                                );
                            } else {
                                Leo.msgfail(res.Msg || "积分或金钱不足！");
                            }
                        })
                    });
                });

                t.InitLikeControlForDiscuz.call(t, 1, p);
                t.TableRemoveStyle();
                t.ChatInit();//初始化聊天
                thisArea.showAgainstUser();
                thisArea.report();
                t.RemoveImageWrapperHref();
                t.ShowMaxImg(".boxtable .plc .pct", 5);
                t.InitShareComponents();
                t.InitClipboard('.btn-paste');
            }
        }
        return function (action, p) {
            return T[action](p);
        }
    },
    article: function () {
        var t = this,
            T = {
                autoSaveIntervalIndex: -1,
                autoSaveItemId: 0,//ID
                cacheKey: function () { return this.autoSaveItemId === 0 ? "ARTICLE_ADD_" + t.user.id : "ARTICLE_EDIT_{0}_{1}".Format(this.autoSaveItemId, t.user.id) },
                /**自动保存*/
                autoSave: function (itemId) {
                    var bbsModule = t.bbs();
                    var thisarea = this;
                    var noEmpty = t.tools.isNotEmptyObject;
                    thisarea.autoSaveItemId = itemId || 0;
                    thisarea.renderOldContent();
                    thisarea.autoSaveIntervalIndex = setInterval(function () {
                        //判断有改变就保存
                        var obj = {
                            isanonymous: bbsModule("isAnonymous"),//是否匿名发布
                            title: $("#articleTitle").val(),//标题
                            tags: bbsModule("addTagsForPost", true),//标签
                            body: encodeURI($("#summernote").summernote("code")),//内容
                            contentNeedFee: $("#articleContentCoinStatus").is(":checked"),//内容付费
                            contentFeeType: $("#articlecontentFeeType").val(),//内容付费类型
                            contentFee: $("#articleContentFee").val(),//内容付费量
                        }
                        if (obj.isanonymous || noEmpty(obj.title) || obj.tags.length > 0
                            || !$("#summernote").summernote("isEmpty") || obj.contentNeedFee
                            || noEmpty(obj.contentFeeType) || noEmpty(obj.contentFee)) {
                            localStorage.setItem(thisarea.cacheKey(), JSON.stringify(obj));
                        } else {
                            localStorage.removeItem(thisarea.cacheKey());
                        }
                    }, 1000);
                },
                /**停止自动保存，并删除已保存的数据 */
                removeAutoSave: function () {
                    clearInterval(this.autoSaveIntervalIndex);
                    localStorage.removeItem(this.cacheKey());
                    this.autoSaveIntervalIndex = -1;
                    this.autoSaveItemId = 0;
                },
                /**加载旧内容 */
                renderOldContent: function () {
                    var content = localStorage.getItem(this.cacheKey())
                    if (!t.tools.isEmptyObject(content)) {
                        t.msg("检测到有您上次编辑未发布内容，已自动还原", null, null, null, 3000)
                        content = JSON.parse(content);
                        console.log(content);
                        $("#add_anonymous").attr("checked", content.isanonymous);//是否匿名发布
                        $("#articleTitle").val(content.title)//标题
                        var taglistwrap = $(".TagListWrap");
                        if (this.autoSaveItemId != 0) {
                            taglistwrap.children().remove();
                        }
                        $.each(content.tags, function (i, tag) {
                            taglistwrap.append($("<span>").addClass("label label-success Lmgr5").attr({ "data-toggle": "tooltip", "title": "点击移除此标签！" }).text(tag[0]).data("value", tag)
                                .click(function () {
                                    //关闭当前添加的标签
                                    $(this).remove();
                                    taglistwrap.find(".tooltip").remove();
                                    taglistwrap.data("TagCount", taglistwrap.children().length);
                                    $("#btn-addtag").removeClass("Ldn");
                                }));
                        });
                        if (content.tags.length == 0) {
                            taglistwrap.next().removeClass("Ldn");
                        }
                        $("#summernote").summernote("code", decodeURI(content.body))//内容
                        if (content.contentNeedFee) {
                            $("#articleContentCoinStatus").attr("checked", content.contentNeedFee)//内容付费
                            $("#articlecontentFeeType").val(content.contentFeeType).attr("disabled", false)//内容付费类型
                            $("#articleContentFee").val(content.contentFee).attr("disabled", false)//内容付费量
                        } else {
                            $("#articlecontentFeeType").val(content.contentFeeType)//内容付费类型
                            $("#articleContentFee").val(content.contentFee)//内容付费量
                        }
                        t.ToolTip();
                    }
                },
                index: function () {
                    t.Search(function (keyword) {
                        var layerIndex = LOAD("正在搜索...");
                        $.get("/article/Search", {
                            key: keyword
                        }, function (data) {
                            if (data.Ok === false) {
                                layer.msg(data.Msg);
                            } else {
                                $(".aw-explore-list .mod-body .aw-common-list").html(data);
                            }
                            CLOSE(layerIndex);
                        });
                    });
                    t.BootStrap_Tab_Change(".aw-common-list", ".aw-common-list", "#Article_Page_Wrap", "/article/PageA");
                },
                publish: function (p) {
                    var thisarea = this;
                    var type, url, desc, isnew = !1, _ischanged = !1, attachCount = 0;
                    var bbsModule = t.bbs();
                    var attachFun = bbsModule("attach", [attachCount, true]);
                    if (t.tools.isNumber(p)) {
                        isnew = !0;
                        type = 1, url = "/article/Publish", desc = "发布";
                    } else if (t.tools.isArray(p)) {
                        type = 0;
                        url = "/article/Edit/{0}".Format(p[1]);
                        desc = "编辑";

                        //获取有几个已有附件
                        attachCount = $(".attachmentwrapper").length - 1;
                    }
                    bbsModule("initAddTag");
                    ////附件相关
                    //function deleteFile(show) {
                    //    var deleteEle = $("#btn-removeFile");
                    //    show && (deleteEle.removeClass("Ldn"));
                    //    deleteEle.UnBindAndBind("click", function () {
                    //        $(this).addClass("Ldn");
                    //        $("#btn-addFile").show();
                    //        var showarea = $("#fileShowArea");
                    //        $("#addFile").val("");
                    //        $("#addFile").data("ischanged", !1);
                    //        isnew || ($("#addFile").data("isdelete", !0));
                    //        showarea.val(""), showarea.hide(), showarea.parent().addClass("Ldn");
                    //    });
                    //}
                    //$("#btn-addFile").UnBindAndBind("click", function () {
                    //    $("#addFile").click();
                    //    $("#fileShowArea").parent().removeClass("Ldn");
                    //    t.tools.PickupInput("#addFile", "#fileShowArea", function () {
                    //        if ($(this).val()) {
                    //            $("#btn-addFile").hide();
                    //            deleteFile(1);
                    //        } else {
                    //            $("#btn-addFile").show();
                    //            $("#btn-removeFile").hide();
                    //        }
                    //    });
                    //});

                    //内容付费
                    $("#articleContentCoinStatus").change(function () {
                        if (this.checked) {
                            $("#articlecontentFeeType").attr("disabled", false);
                            $("#articleContentFee").attr("disabled", false);
                        } else {
                            $("#articlecontentFeeType").attr("disabled", true);
                            $("#articleContentFee").attr("disabled", true);
                        }
                    });

                    //发布
                    $("#btn_publishArt").UnBindAndBind("click", function () {
                        var title = "#articleTitle";
                        if (t.tools.CheckFormNotEmpty(title, "文章标题")) {
                            if (!$('#summernote').summernote("isEmpty")) {

                                //管理付费内容
                                if ($("#articleContentCoinStatus").is(":checked")) {
                                    if (!t.tools.CheckFormNotEmpty($("#articlecontentFeeType"), "内容付费类型")) {
                                        return !1
                                    } else if (!t.tools.CheckFormNotEmpty($("#articleContentFee"), "内容费用")) {
                                        return !1;
                                    }
                                }

                                //管理附件
                                var validate = attachFun.validateAttach();
                                if (validate.ok) {
                                    var _title = title.val();
                                    var layerIndex = layer.load("正在{1}文章：{0}".Format(_title, desc))
                                    var formdata = new FormData();
                                    //var fujian = $("#addFile");
                                    formdata.append("title", $("#articleTitle").val());
                                    formdata.append("body", encodeURI($('#summernote').summernote("code")));
                                    formdata.append("tags", bbsModule("addTagsForPost"));

                                    //添加 是否 匿名发布
                                    formdata.append("isanonymous", bbsModule("isAnonymous"));

                                    //添加内容付费
                                    formdata.append("contentNeedFee", $("#articleContentCoinStatus").is(":checked"));
                                    formdata.append("contentFeeType", $("#articlecontentFeeType").val());
                                    formdata.append("contentFee", $("#articleContentFee").val());

                                    //包装附件
                                    var form_attachIndexs = [];
                                    var attkeys, attachArr = attachFun.getAttachArr();
                                    if (attkeys = Object.keys(attachArr), attkeys.length > 0) {
                                        for (var i in attkeys) {
                                            form_attachIndexs.push(attkeys[i]);
                                            var fujian = $(attachArr[attkeys[i]]["element"]);
                                            if (!t.tools.isEmptyObject(fujian.val())) {
                                                formdata.append("file" + attkeys[i], fujian[0].files[0]);
                                            }
                                        }
                                    }
                                    if (form_attachIndexs.length > 0) {
                                        formdata.append("attachIndexs", form_attachIndexs.join(","));
                                        formdata.append("fees", validate.fees.join(","))
                                    }

                                    ////编辑文章
                                    //if (!isnew) {
                                    //    formdata.append("isdelete", $("#addFile").data("isdelete"));
                                    //    _ischanged = $("#addFile").data("ischanged")
                                    //    formdata.append("ischanged", _ischanged);
                                    //}
                                    t.SendFile2(url, !1, function (data) {
                                        if (data.Ok) {
                                            $("body").empty();
                                            layer.msg(data.Msg || "{0}成功".Format(desc));
                                            layer.close(layerIndex);
                                            thisarea.removeAutoSave();
                                            setTimeout(function () {
                                                layer.load("正在跳转到：{0}".Format(_title));
                                                location.href = data.Url || "/article";
                                            }, 500)
                                        } else {
                                            layer.msg(data.Msg || "文章{0}失败".Format(desc));
                                            layer.close(layerIndex);
                                        }
                                    }, formdata, null, null);
                                }
                            } else {
                                layer.msg("文章内容不能为空");
                                return !1;
                            }
                        }
                    });
                    t.RichText("#summernote", "/Tool/UpLoadIMG", "此处填写文章内容", 330, !1, function (data) {
                        $('#summernote').summernote('insertImage', data.Url);
                    });
                    $("#articleTitle").focus();
                    thisarea.autoSave(isnew ? 0 : p[1]);
                },
                detail: function (p) {
                    var bbsModule = t.bbs();
                    //附件下载
                    $(".buy-attach-btn").click(function () {
                        var me = $(this),
                            fee = me.data("fee"),
                            feetype = me.data("feetype"),
                            aid = me.data("aid"),
                            filename = me.data("filename"),
                            mid = me.data("mid");
                        var confirmIndex = AlertConfirm("确定要花{0}{1}购买 {2} 吗？如若购买请继续！".Format(fee, feetype == 10 ? '积分' : "VIP分", filename), '购买并下载', '再想想', function () {
                            layer.close(confirmIndex);
                            var layerindex = LOAD("正在购买并下载");
                            //判断金钱是否足额
                            $.get("/user/enoughmoney", { fee: fee, feetype: feetype }, function (res) {
                                if (res.Ok) {
                                    layer.close(layerindex);
                                    if (res.Data) {
                                        var url = t.baseUrl + "/down?mt={2}&md={0}&ad={1}".Format(mid, aid, 1);
                                        $(".hasbuyerd_" + aid).hide();
                                        $(".downHref_" + aid).attr({
                                            "href": url,
                                            "target": "_blank"
                                        })
                                        window.open(url, "_blank");
                                    } else {
                                        t.msgfail("对不起，您的余额不足！");
                                    }
                                }
                            })
                        });
                    });

                    //购买主题
                    $(".buycontent").click(function () {
                        var me = $(this),
                            feetype = me.data("feetype"),
                            fee = me.data("fee"),
                            mid = me.data("mid");
                        var lindex = AlertConfirm("确定要花{0}{1}购买主题内容吗？".Format(fee, feetype == 10 ? "积分" : "VIP分"), "购买", "取消", function () {
                            layer.close(lindex);
                            lindex = LOAD("购买中");
                            $.post("/buy", {
                                fee: fee, feetype: feetype, mid: mid, maintype: 2
                            }, function (res) {
                                if (res.Ok) {
                                    t.msgsuccess(res.Msg);
                                    $(".contentLocked").addClass("Ldn").after(
                                        $("<table>").append($("<tr>").append($("<td>").addClass("t_f").html(res.Data)))
                                    );
                                } else {
                                    Leo.msgfail(res.Msg || "积分或金钱不足！");
                                }
                            })
                        });
                    });

                    function Prise() {
                        $(".btn_prise").bind("click", function () {
                            var me = $(this), aid, f = arguments.length > 0;
                            $.post("/bbs/Prise?t=" + t.getPK(), {
                                id: p == (aid = location.pathname.split("/")[3]) ? p : aid, type: 3
                            }, function (rd) {
                                if (rd.Ok) {
                                    me.unbind("click").removeClass("btn_prise").addClass("disabled").text("已点赞").blur();
                                } else {
                                    layer.msg(data.Msg || "赞失败");
                                }
                            });
                        });
                    }

                    $(".down").click(function () {
                        $("<a>").attr({ "href": "/article/DownLoad/{0}".Format(location.pathname.split("/")[3]), "target": "_self" })[0].click();
                    });

                    //Prise();
                    t.InitLikeControlForDiscuz.call(t, 2, p);
                    t.TableRemoveStyle();
                    t.ChatInit();//初始化聊天
                    bbsModule("showAgainstUser");
                    bbsModule("report");
                    t.RemoveImageWrapperHref();
                    t.ShowMaxImg(".boxtable .plc .pct", 5);
                    t.InitShareComponents();
                    t.InitClipboard(".btn-paste");
                },
            };
        return function (action, p) {
            return T[action](p);
        }
    },
    share: function () {
        var t = this,
            check = t.tools.CheckFormNotEmpty,
            T = {
                index: function () {
                    $("#Account").focus();
                    $("#SendEmailCode").click(function () {
                        if (check("#Account", "用户名不能为空")) {
                            var _ = $(this);
                            _.text("发送邮件中");
                            $.post("/tool/ShareRegistEmail?t=" + t.getPK(), {
                                mail: $.trim($("#Account").val()),
                            }, function (data) {
                                if (data.Ok) {
                                    $("#Account").attr("readonly", !0);
                                    t.SendCode.init($("#SendEmailCode"));
                                    $("#btn_register").unbind("click").click(function () {
                                        if (check("#ValidateCode", "邮箱验证码不能为空")) {
                                            if (check("#Password", "密码不能为空")) {
                                                if ($("#Password").val().length >= 6) {
                                                    if ($("#Password").val().length <= 18) {
                                                        if (check("#UserName", "昵称不能为空")) {
                                                            if (check("#Gender", "请选择性别")) {
                                                                if (check("#Province", "请选择省份")) {
                                                                    if (check("#Birth", "出生年月不能为空")) {
                                                                        if (check("#Work", "岗位不能为空")) {
                                                                            $("#nmform").ajaxSubmit(function (data) {
                                                                                if (data.Ok) {
                                                                                    location.href = data.Url || "/";
                                                                                } else {
                                                                                    layer.msg(data.Msg || "注册错误"); return !1;
                                                                                }
                                                                            }, !0);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    } else {
                                                        $("#Password").focus(); layer.msg("密码长度不能大于18位"); return !1;
                                                    }
                                                } else {
                                                    $("#Password").focus(); layer.msg("密码长度不能小于6位"); return !1;
                                                }
                                            }
                                        }
                                    });
                                }
                                else {
                                    _.text("获取邮箱验证码");
                                    $("#Account").val("").focus();
                                    layer.msg(data.Msg);
                                }
                            }).error(function (data) {
                                $(this).text("获取邮箱验证码");
                            });
                        }
                    });
                    $("#UserName").blur(function () {
                        $("#UserName").val($.trim($("#UserName").val()));
                        if (t.tools.CheckFormNotEmpty($("#UserName"), "昵称")) {
                            $.post("/account/ExistNickName", {
                                nickname: $.trim($("#UserName").val())
                            }, function (data) {
                                if (data.toLowerCase() == "true") {
                                    layer.msg("您的昵称已有人使用，请重新更换一个");
                                    $("#UserName").focus();
                                }
                            });
                        }
                    });
                    lay('#Birth').each(function () {
                        laydate.render({
                            elem: this,
                            trigger: 'click',
                            type: 'date',
                            min: "1950-01-01",
                            max: "{0}-12-31".Format(new Date().getFullYear() - 5),
                            calendar: !0,
                            showBottom: !1,
                        });
                    });
                }
            };
        return function (action, p) {
            return T[action](p);
        }

    },
    screenInit: function () {
        //var t = this;
        ////t.Listen["screenInit"] = !1;
        //$("footer").hide();
        //setTimeout(function () {
        //    if ($("body").height() < ($(window).height())) {
        //        $("footer").css({
        //            "position": "fixed",
        //        }).show();
        //    } else {
        //        $("footer").css({
        //            "position": "absolute",
        //        }).show();
        //    }
        //}, 1);
        //if (!t.Listen["screenInit"]) {
        //    t.Listen["screenInit"] = !0;
        //    t.Listen("resize", function () {
        //        t.screenInit.call(t);
        //    });
        //}
    },
    S: function () {
        var t = this;
        t.Page.PageAppend("#search_Page", ".searchResultList", location.href, !0);
        $(".btn-buyOtherItem").UnBindAndBind("click", function () {
            if (t.user.login) {
                var me = $(this);
                AlertConfirm("确定要花{0}积分查看吗？".Format(me.data("c")), '确定', '取消', function () {
                    $.post("/bbs/feeseeother/{0}".Format(me.data("id")), function (res) {
                        if (res.Ok) {
                            t.msgsuccess(res.Msg || "付费成功");
                            location.href = res.Data
                        } else {
                            t.msgfail(res.Msg || "失败");
                        }
                    });
                });
            } else {
                t.gologin();
            }
        });
    },
    tag: function () {
        var t = this,
            T = {
                index: function () {
                    t.BootStrap_Tab_Change(".userTabs", ".tag_all_List", null, "/Tag/Tag?sort={0}", "Sort_All");
                }
            }
        return function (action, p) {
            return T[action](p);
        }
    },
    softlink: function () {
        var t = this;
        var T = {
            index: function () {
                function search() {
                    var loadIndex = LOAD("正在搜索中");
                    var v = $(".searchValue");
                    if (t.tools.CheckFormNotEmpty(v, "搜索内容")) {
                        $.get("/softlink/search", { key: v.val() }, function (data) {
                            layer.close(loadIndex);
                            if (data.Ok) {
                                if (data.Data.length > 0) {
                                    var searchElement = $("<h4>").addClass("Lmgt10").append($("<span>").text("搜索结果").addClass("Lvam"));
                                    var element = $("<div>").addClass("well");
                                    var ul = $("<ul>").addClass("clearfix");
                                    $.each(data.Data, function () {
                                        ul.append($("<li>").addClass("softlinkitem Ltac")
                                            .append($("<a>").attr({
                                                "href": this.url,
                                                "target": "_blank",
                                                "data-toggle": "tooltip",
                                                "title": this.name,
                                            }).text(this.name), $("<p>").attr("title", this.desc).text(this.desc)))
                                    });
                                    $(".softLink").empty().append(searchElement, element.append(ul));
                                    t.ToolTip();
                                } else {
                                    t.msgfail("搜索无结果");
                                }
                            } else {
                                t.msgfail("搜索无结果");
                            }
                        });
                    }
                }
                t.onfocusKeyup($(".searchValue"), t.keyup.onKeyUpType.Enter, function () {
                    search();
                });
                $('.btn-search-softlink').click(function () {
                    search();
                });
            }
        }
        return function (action, p) {
            return T[action](p);
        }
    },
    study: function () {
        var t = this;
        var T = {
            index: function () {
                $(".btn-study").click(function () {
                    var me = this;
                    AlertConfirm("确定学习该课程吗，学习即表示之前学习中的课程已学完，请确认。", "学习", "再看看", function () {
                        $.post("/study/study/{0}".Format($(me).data("id")), function (res) {
                            if (res.Ok) {
                                t.msgsuccess(res.Msg || "学习成功", function () {
                                    //location.reload(!0)
                                    location.href = "/";
                                }, 500);
                            } else {
                                t.msgfail(res.Msg || "学习失败");
                            }
                        });
                    });
                });
                $(".btn-study-finish").click(function () {
                    var me = this;
                    AlertConfirm("确认已学习完成了吗？", "圆满完成", "还没学好", function () {
                        $.post("/study/studyfinish/{0}".Format($(me).data("id")), function (res) {
                            if (res.Ok) {
                                t.msgsuccess(res.Msg || "成功完成学习", function () {
                                    location.href = "/";
                                }, 500);
                            } else {
                                t.msgfail(res.Msg || "失败");
                            }
                        });
                    });
                });
            },
        }
        return function (action, p) {
            return T[action](p);
        }
    },
};
~function ($) {
    var t = this, tools = t.tools = {
    };
    $.each(['String', 'Function', 'Array', 'Number', 'RegExp', 'Object', 'Date'], function (i, value) {
        tools["is" + value] = function (o) {
            return Object.prototype.toString.call(o) == "[object " + value + "]"
        }
    }),
        tools["isEmptyObject"] = function (obj) {
            if (obj == null) return true;
            if (tools.isNumber(obj)) { return false; }
            if (tools.isArray(obj) || tools.isString(obj)) return obj.length === 0;
            for (var key in obj) if (obj.hasOwnProperty(key)) return false;
            return true;
        },
        tools["isNotEmptyObject"] = function (obj) { return !t.tools.isEmptyObject(obj) },
        tools["localUrl"] = function () {
            if (window != window.top) {
                window.top.location = window.location; return !1;
            }
            else {

            }
        },
        tools["buZero"] = function (a) {
            return a > 10 ? a : "0{0}".Format(a);
        },
        tools["GetTime"] = function (format) {
            var date = new Date;
            var year = date.getFullYear();
            var month = this.buZero(date.getMonth() + 1);
            var day = this.buZero(date.getDate());
            var hour = this.buZero(date.getHours());
            var min = this.buZero(date.getMinutes());
            var sec = this.buZero(date.getSeconds());
            return "{0}-{1}-{2} {3}:{4}:{5}".Format(year, month, day, hour, min, sec);
        },
        //time1 和 time2比较，0：时间相同 1：time2大于time1 -1:time1大于time2
        tools["DateCompare"] = function (time1, time2) {
            var oDate1 = new Date(time1).getTime();
            var oDate2 = new Date(time2).getTime();
            return oDate1 == oDate2 ? 0 : oDate1 > oDate2 ? -1 : 1;
        },
        tools["ChangeDateFormat"] = function (cellval) {
            if (cellval != null && cellval != undefined && cellval != "") {
                var date = new Date(parseInt(cellval.replace("/Date(", "").replace(")/", ""), 10));
                var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
                var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
                var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
                var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
                var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
                return date.getFullYear() + "-" + month + "-" + currentDate + " " + hours + ":" + minutes + ":" + seconds;
            }
            else
                return "";
        },
        tools["CheckFormNotEmpty"] = function (item, msg) {
            var me = t.tools.isString(item) ? $(item) : item.length ? item : $(item);
            if (me.length > 0) {
                var i = $.trim(me.val());
                var ok = !0;
                if (i == null || i == undefined | i == "") {
                    ok = !1;
                    var msgMask = Math.floor(Math.random(0, 1) * 10) < 4 ? t.msg : t.msgfail;
                    var _msg;
                    if (me[0].nodeName == "INPUT") {
                        _msg = msg + (msg.indexOf("不能为空") > -1 ? "" : "不能为空");
                    } else if (me[0].nodeName == "SELECT") {
                        _msg = (msg.indexOf("请选择") > -1 ? "" : "请选择") + msg;
                    } else {
                        _msg = msg;
                    }
                    msgMask.call(t, _msg);
                    me.css("border-color", "red");
                    me.focus();
                    me.UnBindAndBind("change input", function () {
                        $(this).css("border-color", "#cccccc");
                    });
                }
                return ok;
            } else {
                alert("缺少必要输入框！");
            }
        },
        tools["PickupInput"] = function (pickupElement, showElement, callback) {
            var pick = $(pickupElement), show = $(showElement);
            pick.change(function () {
                if (pick.val()) {
                    if (show.css("display").indexOf("none") > -1 || show.hasClass("Ldn")) {
                        show.css("display", "block").show();
                        show.hasClass("Ldn") && show.removeClass("Ldn");
                    }
                    show.val(pick.val()); pick.data("ischanged", !0);
                    t.tools.isFunction(callback) && callback.call(show);
                }
            });
            show.on("click focus", function (e) {
                e.preventDefault();
                pick.click();
            })
        },
        tools["SetValueOnSelect"] = function (select, valueElement) {
            $(select).change(function () {
                $(valueElement).val(this.value);
            });
        },
        tools["CheckLength"] = function (obj, minLen, maxLen, msg) {
            var ok = !1;
            if (tools.isString(obj)) {
                if (obj.length < minLen) {
                    layer.msg(msg + "不能小于" + minLen + "个字");
                } else if (obj.length > maxLen) {
                    layer.msg(msg + "不能大于" + maxLen + "个字");
                } else {
                    ok = !0;
                }
            }
            return ok;
        },
        tools["QueryString"] = function (name) {
            var reg = new RegExp('(^|&)' + name + '=([^&]*)(&|$)', 'i');
            var r = window.location.search.substr(1).match(reg);
            if (r != null) {
                return unescape(r[2]);
            }
            return null;
        },
        tools["AddHref"] = function (element) {
            var ele = $(element);
            if (ele.length) {
                var textR = ele.html();
                var reg = /(http:\/\/|https:\/\/)((\w|=|\?|\.|\/|&|-)+)/g;
                var imgSRC = ele.find("img").attr('src');
                if (reg.exec(imgSRC)) {
                    return false
                } else {
                    textR = textR.replace(reg, "<a href='$1$2'>$1$2</a>");
                }
                ele.html(textR);
            }
        }
}.call(this.Leo, $ || jQuery);
+function ($) {
    var t = this;
    t.Listen = function (type, listener, target, useCapture) {
        target = target ? target[0] : window;
        useCapture = useCapture || !1;
        typeof window.addEventListener != "undefined" && target.addEventListener(type, listener, useCapture);
        typeof window.attachEvent != "undefined" && target.attachEvent("on" + type, dt);
    }
}.call(this.Leo, $ || jQuery);
Leo.SendCode = {
    node: null,
    text: null,
    count: 60,
    start: function () {
        if (this.count > 0) {
            this.node.text(this.count-- + "秒后再发送");
            var n = this;
            setTimeout(function () {
                n.start()
            }, 1e3);
        } else {
            this.node.removeAttr("disabled");
            this.node.removeClass("gray");
            this.node.text(this.text || "重新发送");
            this.count = 60;
        }
    },
    init: function (n) {
        this.node = n;
        this.node.attr("disabled", !0);
        this.node.addClass("gray");
        //this.text = this.node.text();
        this.start();
    }
}
Leo.Regpx = {
    //判断是否为正整数
    isNumber: function (s) {//是否为正整数
        var re = /^[0-9]+$/;
        return re.test(s)
    }
}
Leo.Page = function () {
    var t = Leo;
    function Load(btnWrapElement, renderElement, url, removebtnWrapElement, onLoadSuccessCallback) {
        var btn = $(btnWrapElement).find(".btn-pageLoadMore");
        btn && btn.unbind("click").bind("click", function () {
            var me = $(this), pageInfo = me.data("more").split("_"), pi = pageInfo[0], ps = pageInfo[1];
            //var layerIndex = layer.load();
            me.text("正在加载更多数据...").append($('<i class="layui-layer-ico layui-layer-ico16"></i>').css({ width: 40, height: 40, "vertical-align": "middle", display: "inline-block", }));
            $.get(url, {
                pi: pi, ps: ps
            }, function (data) {
                removebtnWrapElement && $(btnWrapElement).remove();
                removebtnWrapElement || me.parent().remove();
                $(renderElement).append(data);
                t.tools.isFunction(onLoadSuccessCallback) && onLoadSuccessCallback();
                //layer.close(layerIndex);
                Load(btnWrapElement, renderElement, url, removebtnWrapElement, onLoadSuccessCallback);
            });
        });
    }
    //分页加载
    function RowNumber(btnWrapElement, renderElement, url, tabsElement, loadWrapper, sortOrOnClickGetSort, callback) {
        var parentElement = $(btnWrapElement),
            pageBtns = parentElement.find(".page");
        pageBtns.length && pageBtns.UnBindAndBind("click", function () {
            if (t.tools.isFunction(sortOrOnClickGetSort)) {
                url = url.Format(sortOrOnClickGetSort());
            } else {
                url = url.Format(sortOrOnClickGetSort);
            }
            var me = $(this), layerIndex;
            t.tools.isFunction(loadWrapper) && (layerIndex = loadWrapper());
            t.ScrollTop(tabsElement);
            setTimeout(function () {
                $.get(url + "{0}pi=".Format(url.indexOf("?") > -1 ? "&" : "?") + me.data("pagerindex"), function (data) {
                    $(renderElement).empty().append(data);
                    try { t.LoadMask.Remove(layerIndex); } catch (e) { layer.close(layerIndex); }
                    t.ToolTip();
                    RowNumber(btnWrapElement, renderElement, url, tabsElement, loadWrapper, sortOrOnClickGetSort, callback);
                    t.tools.isFunction(callback) && callback();
                });
            }, t.tools.Random(500, 1000));
        });
    }
    return {
        //按分页向当前数据下追加更多数据
        PageAppend: Load,
        //按分页切换展示新数据
        PageRowNumber: RowNumber
    };
}();
Leo.msg = function (msg, icon, shift, callback, time) {
    var obj = {}; (icon && (obj.icon = icon)), (shift && (obj.shift = shift)), (callback && (obj.end = function () { callback(); }), (obj.time = time || 1200));
    return layer.msg(msg, obj);
}
Leo.msgsuccess = function (msg, callback, time) {
    return this.msg(msg, this.getPK() % 2 == 0 ? 1 : 6, 1, callback, time)
}
Leo.msgfail = function (msg, callback, time) {
    return this.msg(msg, this.getPK() % 2 == 0 ? 2 : 5, 6, callback, time);
}
Leo.msgwarn = function (msg, callback, time) {
    return this.msg(msg, 7, 7, callback, time);
}
Leo.ScrollTop = function (element) {
    $("body,html").animate({ scrollTop: $(element).offset().top - 50 });
}
Leo.keyup = function (target, type, callback) {
    var t = this, keyup = t.keyup, keyupType = keyup.onKeyUpType;
    $(target).UnBindAndBind("keyup", function (e) {
        if (type === keyupType.Enter) {
            e.keyCode == 13 && t.tools.isFunction(callback) && callback();
        } else if (type === keyupType.Input) {
            t.tools.isFunction(callback) && callback();
        } else if (type === keyupType.Esc) {
            e.keyCode == 27 && t.tools.isFunction(callback) && callback();
        }
    });
}
Leo.keyup.onKeyUpType = {
    Enter: "enter",
    Input: "input",
    Esc: "esc",
}
Leo.onfocusKeyup = function (targetInputForm, keyupType, callback) {
    $(targetInputForm).focus(function () {
        Leo.keyup(document, keyupType, callback);
    }).blur(function () {
        $(document).unbind("keyup");
    })
}
Leo.LoadMask = function (renderWrapElement, loadPosition_Margin, loadPosition_Top) {
    var renderWrap = $(renderWrapElement), height = renderWrap.outerHeight(), width = renderWrap.outerWidth(), maskElement = $("<div>");
    renderWrap.css("position", "relative").append(maskElement.append($("<img>").attr({
        "src": "/Scripts/layer-1.8.5/skin/default/loading-0.gif"
    }).css({ "top": (height - 24) / 2 + "px", "left": (width - 60) / 2 + "px", "position": "absolute" }))
        .css({
            position: "absolute",
            top: loadPosition_Top,
            margin: loadPosition_Margin,
            width: "100%",
            "z-index": "999",
            "user-select": "none",
            "height": "100%",
            "background-color": "rgba(0,0,0,.1)",
            "left": 0, "right": 0,
        }).addClass("loadwrap")
    );
    var index = Leo.LoadMask.length || 0;
    Leo.LoadMask[index] = maskElement;
    return index;
}
Leo.LoadMask.Remove = function (index) {
    Leo.LoadMask[index].remove();
}
$.fn.extend({
    UnBindAndBind: function (event, callback) {
        return this.unbind(event).bind(event, function (e) {
            Leo.tools.isFunction(callback) && callback.call(this, e);
        });
    }
})
$.fn.extend({
    getFormJson: function (frm) {
        var o = {
        };
        var a = $(frm).serializeArray();
        $.each(a, function () {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    },
    ajaxSubmit: function (fn, _validateToken) {
        //将form中的值转换为键值对。
        var that = this, dataPara = this.getFormJson(that),
            uri = that.get(0).action;
        return $.ajax({
            url: uri.indexOf("?") > -1 ? uri + "&t=" + Leo.getPK() : uri + "?t=" + Leo.getPK(),
            type: that.get(0).method,
            data: dataPara,
            success: fn,
            headers: _validateToken ? {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
            } : null
        });
    },
    autoTextarea: function () {
        return $(this).on("input focus blur", function () {
            $(this).each(function () {
                var target = this;
                //保存初始高度，之后需要重新设置一下初始高度，避免只能增高不能减低。           
                var dh = $(target).attr('defaultHeight') || 0;
                if (!dh) {
                    dh = target.clientHeight;
                    $(target).attr('defaultHeight', dh);
                }

                target.style.height = dh + 'px';
                var clientHeight = target.clientHeight;
                var scrollHeight = target.scrollHeight;
                if (clientHeight !== scrollHeight) {
                    target.style.height = scrollHeight + 10 + "px";
                }
            })
        })
    }
});
String.prototype.Format = function (args) {
    var _dic = typeof args === "object" ? args : arguments;
    return this.replace(/\{([^{}]+)\}/g, function (str, key) {
        return _dic.hasOwnProperty(key) ? _dic[key] : str;
    });
}
String.prototype.val = function (args) {
    var ele = (this.indexOf("#") > -1 ? document.getElementById(this.replace("#", "")) : document.getElementsByClassName(this.replace(".", ""))[0]);
    if (args) {
        ele.value = args;
    } else {
        //return ele.nodeName == "INPUT" ? ele.value : "";
        return ele.value || "";
    }
}
Leo.tools.Random = function (Min, Max) {
    var Range = Max - Min;
    var Rand = Math.random();
    var num = Min + Math.round(Rand * Range);
    return num;
}
Leo.browser = {
    versions: function () {
        var u = window.navigator.userAgent;
        return {
            trident: u.indexOf('Trident') > -1, //IE内核
            presto: u.indexOf('Presto') > -1, //opera内核
            webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
            gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, //火狐内核
            //mobile: !!u.match(/AppleWebKit.*Mobile.*/) || !!u.match(/AppleWebKit/), //是否为移动终端
            ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
            android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或者uc浏览器
            iPhone: u.indexOf('iPhone') > -1 || u.indexOf('Mac') > -1, //是否为iPhone或者安卓QQ浏览器
            iPad: u.indexOf('iPad') > -1, //是否为iPad
            webApp: u.indexOf('Safari') == -1,//是否为web应用程序，没有头部与底部
            weixin: u.indexOf('MicroMessenger') == -1 //是否为微信浏览器
        };
    }()
}
Leo.UCenterSkin = function () {
    var t = Leo;
    var maxIndex = 20;
    var skin = [
        { index: "117", title: "白云蓝天", bgcolor: "#c9ceae", },
        { index: "119", title: "黑白画映", bgcolor: "#067caa", },
        { index: "116", title: "画座山给你", bgcolor: "#69a58b", },
        { index: "114", title: "蓝", bgcolor: "#b5e2ff", },
        { index: "113", title: "蓝天下的草帽", bgcolor: "#526102", },
        { index: "112", title: "水天一色", bgcolor: "#6cb0c5", },
        { index: "111", title: "雾的都市", bgcolor: "#000103", },
        { index: "110", title: "相思红豆", bgcolor: "#000154", },
        { index: "109", title: "辽阔草原", bgcolor: "#bca847", },
        { index: "108", title: "冷酷的面庞", bgcolor: "#ccebff", },
        { index: "107", title: "星空", bgcolor: "#387ba5", },
        { index: "106", title: "极夜下的欢舞", bgcolor: "#000", },
        { index: "105", title: "摩天轮", bgcolor: "#000", },
        { index: "104", title: "夕阳下的缭绕", bgcolor: "#322f3a", },
        { index: "103", title: "魅力hold住", bgcolor: "#281B2D", },
        { index: "102", title: "烟雾弥漫", bgcolor: "#020001", },
        { index: "101", title: "绚丽光影", bgcolor: "#911f00", },
        { index: "100", title: "想唱就唱", bgcolor: "#f7b72e", },
        { index: "99", title: "线条之舞", bgcolor: "#f19299", },
        { index: "98", title: "夏", bgcolor: "#3e8b17", },
        { index: "97", title: "雾中的城堡", bgcolor: "#886b5b", },
        { index: "96", title: "外太空", bgcolor: "#17124c", },
        { index: "95", title: "藤蔓", bgcolor: "#fee3ea", },
        { index: "94", title: "太阳花", bgcolor: "#0070aa", },
        { index: "93", title: "秋天的剪影", bgcolor: "#136fb5", },
        { index: "92", title: "蒲公英", bgcolor: "#509529", },
        { index: "91", title: "迷失", bgcolor: "#020001", },
        { index: "90", title: "萌芽", bgcolor: "#1375a1", },
        { index: "89", title: "咖啡时刻", bgcolor: "#300f01", },
        { index: "88", title: "简约", bgcolor: "#5b2712", },
        { index: "87", title: "激光绚烂", bgcolor: "#800753", },
        { index: "86", title: "灰色线条", bgcolor: "#ffffff", },
        { index: "85", title: "欢乐时光", bgcolor: "#eee1c0", },
        { index: "84", title: "春意盎然", bgcolor: "#addb7f", },
        { index: "83", title: "初冬", bgcolor: "#d7bb75", },
        { index: "82", title: "抽象", bgcolor: "#228d84", },
        { index: "77", title: "红色圣诞", bgcolor: "#dd2429", },
        { index: "75", title: "绿色圣诞", bgcolor: "#4e7c25", },
        { index: "74", title: "童年记忆", bgcolor: "#95F1FF", },
        { index: "73", title: "彩色之手", bgcolor: "#6C4207", },
        { index: "72", title: "幸福小屋", bgcolor: "#e0e0e0", },
        { index: "71", title: "粉色点点", bgcolor: "#ec96b1", },
        { index: "70", title: "风花雪兔", bgcolor: "#95DBE7", },
        { index: "69", title: "鱼的世界", bgcolor: "#0288c5", },
        { index: "68", title: "遥望雪山", bgcolor: "#e0e0e0", },
        { index: "67", title: "快乐火车", bgcolor: "#BAF3F4", },
        { index: "66", title: "炫动火花", bgcolor: "#8B65A1", },
        { index: "65", title: "黑风谷", bgcolor: "#001118", },
        { index: "64", title: "苹果乐园", bgcolor: "#70D4E4", },
        { index: "63", title: "守候", bgcolor: "#FCA32F", },
        { index: "62", title: "舞动奇迹", bgcolor: "#501e67", },
        { index: "61", title: "绿色果树", bgcolor: "#95fcc5", },
        { index: "60", title: "那年.那时", bgcolor: "#462419", },
        { index: "59", title: "灰色冰肌", bgcolor: "#303030", },
        { index: "58", title: "少女情怀", bgcolor: "#004D2F", },
        { index: "57", title: "小花猫", bgcolor: "#FCB6C0", },
        { index: "56", title: "暮色灯谜", bgcolor: "#3D6684", },
        { index: "55", title: "无法取代", bgcolor: "#FCA32F", },
        { index: "54", title: "我是NO.1", bgcolor: "#73c44b", },
        { index: "53", title: "极速光线", bgcolor: "#407087", },
        { index: "52", title: "爱的等待", bgcolor: "#b6b6b6", },
        { index: "51", title: "安静的灵魂", bgcolor: "#b6b6b6", },
        { index: "50", title: "欧洲花藤", bgcolor: "#40AD96", },
        { index: "49", title: "人体色彩", bgcolor: "#4b1d68", },
        { index: "48", title: "我住月球", bgcolor: "#5CBEFF", },
        { index: "47", title: "圣女果", bgcolor: "#C0AFC6", },
        { index: "46", title: "生命挑战", bgcolor: "#141517", },
        { index: "31", title: "蓝帅", bgcolor: "#141517", },
        { index: "30", title: "爱情小象", bgcolor: "#FCA32F", },
        { index: "29", title: "水墨小人", bgcolor: "#e0e0e0", },
        { index: "28", title: "暴力使者", bgcolor: "#141517", },
        { index: "27", title: "钢铁战士", bgcolor: "#141517", },
        { index: "26", title: "童话世界", bgcolor: "#72C54A", },
        { index: "25", title: "蓝色幻想", bgcolor: "#064474", },
        { index: "24", title: "空中飞鱼", bgcolor: "#5C5081", },
        { index: "23", title: "飞舞蜻蜓", bgcolor: "#BC7B21", },
        { index: "22", title: "彩色气球", bgcolor: "#5cbeff", },
        { index: "21", title: "幻彩空间", bgcolor: "#320053", },
        { index: "20", title: "秋色剪影", bgcolor: "#9C320F", },
        { index: "19", title: "爱心大脸", bgcolor: "#FCB6C0", },
        { index: "18", title: "灰色经典", bgcolor: "#e0e0e0", },
        { index: "16", title: "绿色经典", bgcolor: "#B4E19C", },
        { index: "15", title: "黄色经典", bgcolor: "#FBE7B3", },
        { index: "14", title: "紫色经典", bgcolor: "#e6cdf5", },
        { index: "13", title: "粉色经典", bgcolor: "#ffaac7", },
        { index: "1", title: "蓝色经典", bgcolor: "#bde2f8", },
    ], thumbPath = "/Content/img/skin/thumb/{0}.jpg", skinPath = "/Content/img/skin/{0}.jpg";
    function Run() {
        var width = $("body").width();
        width = 510 < width ? 510 : width * 0.9;
        $('.skinsetbtn').UnBindAndBind("click", function () {
            var render = $("#skinList").find("ul"), oldskin = $("body").attr("class"), selectedskin, oldli, isset = !1;
            $.each(skin, function (i, n) {
                if (i < maxIndex) {
                    var imgEle = $("<img>").attr({ "src": thumbPath.Format(skin[i].index), "title": "点击预览：{0}".Format(skin[i].title) }).click(function () {
                        selectedskin = "skin-{0}".Format(skin[i].index);
                        $("body").removeAttr("class").addClass("userbg {0}".Format(selectedskin));
                        oldli && oldli.css("border", "1px solid #fff");
                        liEle.css("border", "1px solid red");
                        oldli = liEle;
                    }),
                        pEle = $("<p>").text(skin[i].title);
                    var liEle = $("<li>").css("border", "1px solid #fff").append(imgEle, pEle);
                    render.append(liEle);
                } else {
                    return;
                }
            });
            //$("body").css({ "overflow": "hidden" });
            AlertActionAreaWithConfirmWithSize($("#skinList"), "皮肤设置", width + "px", "300px", '确定设置', null, function () {
                $.post("/user/skin", { skin: selectedskin }, function (data) {
                    if (data.Ok) {
                        layer.closeAll();
                        isset = !0;
                        t.msgsuccess("更换皮肤成功！");
                        $("body").css("overflow", "auto");
                        $("body").removeAttr("class").addClass("userbg {0}".Format(selectedskin));
                        selectedskin = null;
                        oldli = null;
                        render.empty();
                    } else {
                        t.msgfail(data.Msg || "更换皮肤失败！");
                    }
                });
            }, function () {
                //$("body").css("overflow", "auto");
                if (!isset) {
                    $("body").removeAttr("class").addClass(oldskin);
                }
                render.empty();
            })
        });
    }
    return function () {
        Run();
    }
}();

/**
 * 广告位
 */
Leo.AD = function (msgs) {
    var winWidth = $(document).width();
    function render(msgs) {
        //移动端不显示
        var browser = Leo.browser;
        if (!(browser.versions.ios || browser.versions.android || browser.versions.iPhone || browser.versions.iPad)) {
            //单击显示随机文字
            var a_idx = 0, denyClick = !1, usecontainer = true,
                noshowarr = ["button", "a", "input", "textarea", "img", 'select'],
                noshowClassNames = ["layui-layer-shade", "layui-layer-content layui-layer-padding"],
                noshowIdNames = [],
                denyRoutes = {
                    home: [],
                    user: []
                };
            jQuery(document).ready(function ($) {
                $(document).mousedown(function (e) {
                    if (!denyClick) {
                        var sourceElement = e.target || e.currentTarget;
                        var ok = true;
                        if (usecontainer) {
                            if ($(sourceElement).parents(".container").length > 0) {
                                ok = false;
                            } else {
                                if (noshowarr.indexOf(sourceElement.nodeName.toLowerCase()) == -1) {
                                    if (noshowClassNames.indexOf(sourceElement.className) == -1) {
                                        if (noshowIdNames.indexOf(sourceElement.id) == -1) {
                                            ok = true;
                                        } else {
                                            ok = false;
                                        }
                                    } else {
                                        ok = false;
                                    }
                                } else {
                                    ok = false;
                                }
                            }
                        }
                        else {
                            if (noshowarr.indexOf(sourceElement.nodeName.toLowerCase()) == -1 && $(sourceElement).parent()[0].nodeName.toLowerCase() != "a") {
                                ok = true;
                            } else {
                                ok = false;
                            }
                        }
                        if (ok) {
                            denyClick = !0;
                            var a = msgs.length > 0 ? msgs : new Array("路见不平一声吼，吼完继续往前走。", "咸鱼翻身，还是咸鱼。", "水能载舟，亦能煮粥！", "明月几时有，抬头自己瞅。", "天哪！我的衣服又瘦了。", "听君一席话，回家烤白薯。", "流氓不可怕，就怕流氓有文化。", "知识就像内裤，看不见但很重要。", "为了祖国下一代，再丑也得谈恋爱。", "穷玩车，富玩表，牛B加班敲电脑。", "英雄不问出路，流氓不看岁数。", "我们的目标：向钱看，向厚赚。", "帅有个屁用！到头来还不是被卒吃掉！");
                            var item = a[a_idx];
                            var $i;
                            if (Leo.tools.isObject(item)) {
                                $i = $("<span>").html(a[a_idx].msg).css({ "color": a[a_idx].color });
                                $i.children().css({ "color": a[a_idx].color });
                                $i.find("a").css({ "color": a[a_idx].color, "text-decoration": "underline {0}".Format(a[a_idx].color) })
                            } else {
                                $i = $("<span>").html(a[a_idx])
                                    .css({ "color": "#FF69B4", display: 'none' });
                            }
                            $("body").append($i);
                            a_idx = (a_idx + 1) % a.length;
                            var x = e.pageX, y = e.pageY;
                            var maxWidth = 200;
                            var currentTextWidth = $i.width();
                            currentTextWidth = currentTextWidth < maxWidth ? currentTextWidth : maxWidth;
                            var currentTextCSS = {
                                "z-index": 1e10,
                                "top": y - 30,
                                //"left": x,
                                "position": "absolute",
                                "font-weight": "bold",
                                "user-select": "none",
                                "display": "block",
                            };
                            if (winWidth - x - 10 >= currentTextWidth) {
                                currentTextCSS["left"] = x;
                            } else {
                                currentTextCSS["right"] = winWidth - x;
                            }
                            if (item.msg.indexOf("</a>") > -1) {
                                currentTextCSS["cursor"] = "pointer";
                            }
                            $i.css(currentTextCSS);
                            $i.animate({ "top": y - 180, "opacity": 0 }, 3000, function () { $i.remove(); });
                            $i.mouseover(function () {
                                if (parseFloat($i.css("opacity")) > 0.6) {
                                    $i.stop(!0);
                                }
                            }).mouseleave(function () { $i.animate({ "top": y - 180, "opacity": 0 }, 3000, function () { $i.remove(); }); });
                            denyClick = !1;
                            //setTimeout(function () { denyClick = !1; }, 250)
                        }
                    }
                });
            });
        }
    }
    render(msgs);
    ////从缓存取数据
    //var key = "BXT_AD_CACHE_KEY";
    //var items = localStorage.getItem(key);
    //var ok = false;
    //if (!items) {
    //    ok = false;
    //} else {
    //    items = JSON.parse(items);
    //    ok = items.time > Leo._pk
    //}
    //if (!ok) {
    //    $.get("/common/clickmsgs", function (data) {
    //        if (data.Ok) {
    //            render(data.Data);
    //            localStorage.setItem(key, JSON.stringify({
    //                time: +new Date() + (24 * 60 * 60 * 1000),//1天内不更新
    //                data: data.Data,
    //            }));
    //        }
    //    })
    //} else {
    //    render(items.data);
    //}
}

/**分享按钮组件化 */
Leo.InitShareComponents = function () {
    //Leo.RenderCSS("/content/share/dist/css/share.min.css");
    //Leo.RenderScript('/content/share/dist/js/social-share.min.js');
    //setTimeout(function () {
    var $config = {
        //url: "http://www.baidu.com",
        //wechatQrcodeTitle: "微信扫一扫：分享", // 微信二维码提示文字
        //wechatQrcodeHelper: '<p>微信里点“发现”，扫一下</p><p>二维码便可将本文分享至朋友圈。</p>',
        sites: ["wechat", "qq", "qzone", "weibo",],
        mobileSites: ["qq"],
    };
    socialShare('.social-share-cs', $config);
    //}, 888)
}

/**初始化复制组件 */
Leo.InitClipboard = function (element) {
    //复制
    var clipboard = new ClipboardJS(element);
    clipboard.on('success', function (e) {
        Leo.msgsuccess("复制成功")
        e.clearSelection();
    });
    clipboard.on('error', function (e) {
        Leo.msgfail("无法复制，请手动复制");
    });
}

/**引入js脚本 */
Leo.RenderScript = function (javascriptSrc) {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = javascriptSrc;
    head.appendChild(script);
}

/**引入CSS样式脚本 */
Leo.RenderCSS = function (cssHref) {
    var style = document.createElement('link');
    style.href = cssHref;
    style.rel = 'stylesheet';
    style.type = 'text/css';
    document.getElementsByTagName('head').item(0).appendChild(style);
}

/* 脚本最后执行*/
$(document).ready(function () {
    Leo.InitBodyTop();
    Leo.InitGoTopButton();
    //setTimeout(function () {
    //    Leo.AD();
    //}, 3000);
});
