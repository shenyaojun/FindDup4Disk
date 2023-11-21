<script>
import { defineComponent } from 'vue'
import { ElButton } from 'element-plus'

export default defineComponent({
  components: {
    ElButton,
    //ElConfigProvider
  },
  setup() {

    const clickme = async () => {  
      
      alert("aaaa")
      window.chrome.webview.hostObjects.webBrowserObj.ShowMessage(); 
      // alert("bbb")
      // var hostObject = window.chrome.webview.hostObjects.customWebView2HostObject;
      // var result = await hostObject.TestCallFromJS();
      // alert(result)
    };  
    const clickme2 = async () => {  
      
      alert("bbbb")
      window.chrome.webview.hostObjects.webBrowserObj.ShowMessage(); 
      // alert("bbb")
      // var hostObject = window.chrome.webview.hostObjects.customWebView2HostObject;
      // var result = await hostObject.TestCallFromJS();
      // alert(result)
    };  
    const clickme3 = async () => {  
      
      alert("cccc")
      window.chrome.webview.hostObjects.webBrowserObj.ShowMessage(); 
      // alert("bbb")
      // var hostObject = window.chrome.webview.hostObjects.customWebView2HostObject;
      // var result = await hostObject.TestCallFromJS();
      // alert(result)
    };  
  
    return {  
      clickme,  
      clickme2,  
      clickme3, 
    };  

    // return {
    //   zIndex: 3000,
    //   size: 'large',
    // }
  },
})
</script>
<template>
  
  <el-button type="primary" @click="clickme()" class="btn">MD5单磁盘扫描</el-button>
  <el-button type="success" @click="clickme2()" class="btn">单磁盘重复排查</el-button>
  <el-button type="danger" @click="clickme3()" class="btn">去扫描另一台新机器</el-button>
</template>

<style scoped>
body {
    margin: 0;
    display: flex;
    place-items: center;
    min-width: 320px;
}

#app {
    max-width: 1280px;
    margin: 0 auto;
    padding: 0rem;
    padding-top: 0rem;
    padding-right: 2rem;
    padding-bottom: 0rem;
    padding-left: 2rem;
    text-align: center;
}
.btn{
  height:  50px;
  font-size:medium;
  margin-left: 20px;
  margin-right: 20px;
  }
</style>
