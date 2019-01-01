// var cookie=document.cookie;
// document.getElementsByClassName("password").ekey.value=将军令值
// document.getElementsByClassName("btn-wrap btn-wrap-1")[0].getElementsByClassName("longBtn")[0].click()
 $.ajax({
            "url" : "https://2y155s0805.51mypc.cn:12895/",    //提交URL
            "type" : "Get",//处理方式
            //"data" : "username=" + username,//提交的数据
            //"dataType" : "text",//指定返回的数据格式
            "success" : function(data){
            	console.log("请求成功:");
            	console.log(data);
            },//执行成功后的回调函数
            "async" : "false",//是否同步
            //错误后执行
            "error" : function(msg) {
                console.error(msg);
            }

        });
