/**
 * Node-RED IoT数据采集平台配置文件
 * 用于服务端规则引擎和数据流处理
 */

module.exports = {
  // Node-RED运行端口
  uiPort: process.env.PORT || 1880,

  // Node-RED安全配置
  adminAuth: {
    type: "credentials",
    users: [{
      username: "admin",
      password: "$2b$08$zZWtXTja0fB1pzD4sHCMyOCMYz2Z6dNbM6tl8sJogENOMcxWV9DN.", // "iot123456"
      permissions: "*"
    }]
  },

  // HTTP静态文件目录
  httpStatic: '/usr/src/node-red/static/',

  // 启用项目功能
  projects: {
    enabled: true,
    workflow: {
      mode: "manual"
    }
  },

  // 编辑器设置
  editorTheme: {
    projects: {
      enabled: true,
      workflow: {
        mode: "manual"
      }
    },
    palette: {
      categories: ['subflows', 'common', 'function', 'network', 'sequence', 'parser', 'storage']
    },
    codeEditor: {
      lib: "ace",
      options: {
        theme: "vs-dark",
        fontSize: 14,
        fontFamily: "Cascadia Code, Consolas, 'courier new', monospace",
        scrollPastEnd: 0.5,
        behavioursEnabled: true,
        wrapBehavioursEnabled: true,
        autoScrollEditorIntoView: true,
        copyWithEmptySelection: false,
        useSoftTabs: true,
        navigateWithinSoftTabs: false,
        enableMultiselect: true
      }
    }
  },

  // 功能全局配置
  functionGlobalContext: {
    mysql: require('mysql2'),
    axios: require('axios'),
    moment: require('moment'),
    // IoT项目特定配置
    iotConfig: {
      mysql: {
        host: 'iot-mysql',
        port: 3306,
        user: 'root',
        password: 'iot123456',
        database: 'IoTDataCollection_Main'
      },
      api: {
        baseUrl: 'http://host.docker.internal:44330',
        timeout: 30000
      }
    }
  },

  // 日志设置
  logging: {
    console: {
      level: "info",
      metrics: false,
      audit: false
    }
  },

  // 导出全局模块
  exportGlobalContextKeys: false,

  // 上下文存储
  contextStorage: {
    default: "memory",
    memory: { module: "memory" }
  },

  // 调试设置
  debugMaxLength: 1000,

  // 节点相关设置
  nodeTimeout: 30000,

  // MQTT设置（为将来使用预留）
  mqttReconnectTime: 15000,
  serialReconnectTime: 15000
}; 