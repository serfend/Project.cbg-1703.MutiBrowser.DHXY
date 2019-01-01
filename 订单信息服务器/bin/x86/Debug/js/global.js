//"t" ==> window
//"v" ==> window.navigator?
//"w" ==> 配置

console.log("加载global.js");
var window={
	Watchman:{},
	navigator:{
		appCodeName:"Mozilla",
		appName:"Netscape",
		appVersion:"5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36",
		budget:{},
		connection:{},
		cookieEnabled:true,
		deviceMemory:8,
		doNotTrack:null,
		language:["zh-CN","zh"],
		userAgent:"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36"
	},
	document:{
		getElementById:function(element){
			console.log("document.getElementById(" + element +") is calling" );
			return "defaultElement";
		}
	},
	location:{
		hostname:"epay.163.com"
	},
	localStorage:{
		setItem:function(key,value){
			console.setStorage(key,value);
		},
		getItem:function(key){
			return console.getStorage(key);
		}
	},
	screen:{
		colorDepth:24
	},
	Date:function(){

	},
	userHttpRequest:function(){
		
		
	}
};
window.Date.prototype.getTime=function(){
			return console.getTime();
		}
window.Date.prototype.getTimezoneOffset=function (){
			return -480;
		}
window.localStorage.setItem("WM_DID","ltJVwcXNbmtBEFEFFBdpLq3uXyzDbdP/__1545978594567__1545906594567");
var instanceTmp={};