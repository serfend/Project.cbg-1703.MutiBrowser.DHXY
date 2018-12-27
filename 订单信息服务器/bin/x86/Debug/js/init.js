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
	  