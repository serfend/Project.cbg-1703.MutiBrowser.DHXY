// var cookie=document.cookie;
// document.getElementsByClassName("password").ekey.value=将军令值
// document.getElementsByClassName("btn-wrap btn-wrap-1")[0].getElementsByClassName("longBtn")[0].click()
// 创建一个Socket实例
var socket = new WebSocket('wss://2y155s0805.51mypc.cn'); 
 
// 打开Socket
socket.onopen = function(event) { 
  // 监听消息
  socket.onmessage = function(event) { 
    if(event.data){
    	var msg=event.data;
    	console.log("来自服务器的信息"+msg);
    }
  }; 
 
  // 监听Socket的关闭
  socket.onclose = function(event) { 
    console.log('Client notified socket has closed',event); 
  }; 
 
  // 关闭Socket....
  //socket.close()
};