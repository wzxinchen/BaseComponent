var uploader = new plupload.Uploader({ //实例化一个plupload上传对象
    browse_button: '_btnSelectFile',
    url: '{uploadUrl}',
    flash_swf_url: 'js/Moxie.swf',
    silverlight_xap_url: 'js/Moxie.xap',
    prevent_duplicates: true,
    filters:
         [ //只允许上传图片文件
          { title: '图片文件', extensions: 'jpg,gif,png' }
         ]
});
uploader.init(); //初始化
uploader.bind('FilesAdded', function (uploader, files) {
    for (var i = 0, len = files.length; i < len; i++) {
        previewImage(files[i], function (imgsrc, file) {
            App._panelImages.add(Ext.create("Ext.panel.Panel", {
                width: 100,
                height: 100,
                margin: "5",
                title: file.name,
                closable: true,
                items: [
                    Ext.create("Ext.Img", {
                        width: 100,
                        height: 55,
                        src: imgsrc,
                        listeners: {
                            el: {
                                dblclick: function () {
                                    window.open(imgsrc, "_blank");
                                }
                            }
                        }
                    }),
                    Ext.create("Ext.ProgressBar", {
                        width: 100,
                        height: 20,
                        text: "等待上传",
                        id: "bar" + file.id,
                        border: false
                    })
                ],
                listeners: {
                    beforedestroy: function () {
                        uploader.removeFile(file);
                        window.removeFile(file.url);
                    }
                }
            }));
        })
    };
});
var hdnField = Ext.getCmp("{fieldName}");
window.removeFile = function (url) {
    if (url) {
        var value = hdnField.getValue();
        if (value) {
            var arr = value.split(',');
            Ext.Array.remove(arr, url);
            hdnField.setValue(arr.join(","));
        }
    }
}

window.addFileUrl = function (url) {
    if (url) {
        var value = hdnField.getValue();
        var arr;
        if (!value) { arr = []; }
        else {
            arr = value.split(',');
        }
        arr.push(url);
        hdnField.setValue(arr.join(","));
    }
}

window.updateFileUrl = function () {
    App._panelImages.removeAll();
    var imgUrls = hdnField.getValue();
    if (imgUrls == "") return;
    var imgs = imgUrls.split(',');
    for (var i = 0; i < imgs.length; i++) {
        var imgsrc = imgs[i];
        App._panelImages.add(Ext.create("Ext.panel.Panel", {
            width: 100,
            height: 100,
            margin: "5",
            title: "图片" + (i + 1),
            closable: true,
            items: [
                Ext.create("Ext.Img", {
                    width: 100,
                    height: 55,
                    src: imgsrc,
                    listeners: {
                        el: {
                            dblclick: function () {
                                window.open(imgsrc, "_blank");
                            }
                        }
                    }
                })
            ]
        }));
    }
}

uploader.bind('UploadProgress', function (uploader, file) {
    var percent = file.percent / 100;
    Ext.getCmp("bar" + file.id).updateProgress(percent, "正在上传", true);
});
uploader.bind('FileUploaded', function (uploader, file, responseObject) {
    var percent = file.percent / 100;
    Ext.getCmp("bar" + file.id).updateProgress(1, "上传完毕", true);
    var resp = responseObject.response;
    resp = eval("(" + resp + ")");
    window.addFileUrl(resp.message);
});
//plupload中为我们提供了mOxie对象
//有关mOxie的介绍和说明请看：https://github.com/moxiecode/moxie/wiki/API
//如果你不想了解那么多的话，那就照抄本示例的代码来得到预览的图片吧
function previewImage(file, callback) {//file为plupload事件监听函数参数中的file对象,callback为预览图片准备完成的回调函数
    if (!file || !/image\//.test(file.type)) return; //确保文件是图片
    if (file.type == 'image/gif') {//gif使用FileReader进行预览,因为mOxie.Image只支持jpg和png
        var fr = new mOxie.FileReader();
        fr.onload = function () {
            callback(fr.result, file);
            fr.destroy();
            fr = null;
        }
        fr.readAsDataURL(file.getSource());
    } else {
        var preloader = new mOxie.Image();
        preloader.onload = function () {
            //preloader.downsize(100, 100);//先压缩一下要预览的图片,宽300，高300
            var imgsrc = preloader.type == 'image/jpeg' ? preloader.getAsDataURL('image/jpeg', 80) : preloader.getAsDataURL(); //得到图片src,实质为一个base64编码的数据
            callback && callback(imgsrc, file); //callback传入的参数为预览图片的url
            preloader.destroy();
            preloader = null;
        };
        preloader.load(file.getSource());
    }
}

window.btnRemoveAll_click = function () {
    App._panelImages.removeAll();
}
window.startUpload = function () {
    uploader.start();
};