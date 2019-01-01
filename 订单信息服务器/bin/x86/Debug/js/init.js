console.log("加载init.js");
window.initWatchman({
        productNumber: 'YD00000595128763', // 产品编号
        onload: function (instance) {
			console.log("watchManOnload()");
			instanceTmp = instance
        },
        onerror: function (e) {
        }
      })

//document.getElementsByClassName("password").ekey.value=将军令值
//document.getElementsByClassName("btn-wrap btn-wrap-1")[0].getElementsByClassName("longBtn")[0].click()
//提交将军令按钮