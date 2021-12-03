# 環境
ご参考までに私の環境は以下の通りです。
|用途|O/S|ホストタイプ|IP|
|:--|:--|:--|:--|
|クライアントPC|Windows10 Pro|物理ホスト|192.168.11.5/24|
|Docker環境|Ubuntsu 20.04.1 LTS|上記Windows10上の仮想ホスト(vmware)|192.168.11.48/24|

Ubuntsuは、[ubuntu-20.04.1-live-server-amd64.iso](http://old-releases.ubuntu.com/releases/20.04.1/ubuntu-20.04.1-live-server-amd64.iso)を使用して、最低限のサーバ機能のみをインストールしました。

また、Windowsのhostsファイルに、irishostを登録しています。 
```
192.168.11.48 irishost
```

# 起動方法
```
$ git clone https://github.com/IRISMeister/Samples-MQTT-EKG-Devices.git
$ cd Samples-MQTT-EKG-Devices
$ ./setup.sh
$ docker-compose up -d
```
# 停止(削除)方法
```
$ docker-compose down
```

# MQTT受信用のビジネスサービスについて

|BS|送信先|備考|
|:--|:--|:--|
|From_MQTT_EXT|Process_MQTT|External Language明示使用。下記PEX利用を推奨|
|From_MQTT_PEX|Process_MQTT|PEX使用。配列を完全にリレーショナル化する例|
|From_MQTT_PEX2|Process_MQTT|PEX使用。シリアライズ(文字列化)した配列を使用、応答メッセージをIRISで作成する例|
|From_MQTT_PEX3|Process_MQTT|PEX使用。シリアライズ(文字列化)した配列を使用、応答メッセージをPEXで作成する例|
|From_MQTT_PT|Decode_MQTT_PEX|標準のPassThroughサービスを使用|

# データの送信方法
## コマンドライン
```
$ docker-compose exec iris mosquitto_pub -h "mqttbroker" -p 1883 -t /ID_123/XGH/EKG/PT -m "90"
```
上記コマンドを実行すると、From_MQTT_PTがMQTTメッセージを受信し、その後の処理が実行されます。

ただし、各BSは以下のTopicをSubscribeする設定になっています。
|受信するBS|Topic|
|:--|:--|
|From_MQTT_EXT|/ID_123/XGH/EKG/EXT|  
|From_MQTT_PEX|/ID_123/XGH/EKG/PEX|
|From_MQTT_PEX2|/ID_123/XGH/EKG/PEX2|
|From_MQTT_PEX3|/ID_123/XGH/EKG/PEX3|
|From_MQTT_PT|/ID_123/XGH/EKG/PT|



必要に応じてサブスクライブも可能です。
```
$ docker-compose exec iris mosquitto_sub -v -h "mqttbroker" -p 1883 -t /ID_123/XGH/EKG/PT/#
```


## (バイナリ)ファイルを送る方法
バイナリファイルを下記で作成します。
[example.py](datavol/share/example.py)はavroエンコードされたファイルを作成します。[testdata.py](datavol/share/testdata.py)は単純なlong型の配列です。
```
$ docker-compose exec python bash
root@d20238018cbc:~# cd share/
root@d20238018cbc:~/share# python example.py
root@d20238018cbc:~/share# python testdata.py
```

```
# mosquitto_pub -h "mqttbroker" -p 1883 -t /ID_123/XGH/EKG/PT -f /home/irisowner/share/example.avro
```

# その他
## MQTTクライアント機能を直接使用する方法
```
$ docker-compose exec iris iris session iris
USER>set m=##class(%Net.MQTT.Client).%New("tcp://mqttbroker:1883")
USER>set tSC=m.Connect()
USER>set tSC=m.Subscribe("/ID_123/XGH/EKG/PT/#")
USER>set tSC=m.Receive(.topic,.message,10000)
USER>w topic
/ID_123/XGH/EKG/PT
USER>w message
90
USER>h
$
```

## PEXを使わずに.NETを呼び出す方法
```
$ docker-compose exec iris iris session iris
USER>w $SYSTEM.external.getRemoteGateway("netgw",55556).new("System.DateTime",0).Now
2021-11-19 03:51:03.4967784

USER>Set gw=$SYSTEM.external.getRemoteGateway("netgw",55556)
USER>do gw.addToPath("/app/MyLibrary.dll")
USER>set proxy = gw.new("dc.MyLibrary")
USER>w proxy.GetNumber()
123
```
Windows(非コンテナ環境)の場合も同様。事前にExternal Language Serverの%DotNet Serverを起動しておくこと。  
[SMP][システム管理][構成][接続性][External Language Servers]
```
USER>set gw=$SYSTEM.external.getRemoteGateway("localhost",53372)
USER>do gw.addToPath("C:\Users\irismeister\Source\repos\adonet\MyLibrary\bin\Release\MyLibrary.dll")
```
## Genericタイプ
ラッパー経由でのアクセスになります。
```
USER>set proxy = gw.new("dc.GenericList")
<THROW>%Constructor+25^%Net.Remote.Object.1 *%Net.Remote.Exception <GATEWAY> System.Exception InterSystems.Data.IRISClient.Gateway.Gateway.loadClass(String?className) Class not found: dc+GenericList
USER 4e1>q
USER>set proxy = gw.new("dc.GenericListWrapper")
USER>set l = proxy.GetIntList()
USER>d lint.Add(1)
1

USER>d lint.Add(2)
2
USER>set lstr = proxy.GetStringList()
USER>d lstr.Add("abc")
abc

USER>d lstr.Add("日本語")
日本語
```
## .NETアプリケーションを呼び出す方法
```
$ docker-compose exec netgw bash
root@f718a9177d25:/app# dotnet myapp.dll
```

## 参考にしたコードサンプル
https://github.com/intersystems/Samples-PEX-Course

## mosquittoを自前でビルドする
ネット上のブローカmqtt.eclipseprojects.ioはwebsocketが有効になっているが、コンテナ提供されているイメージはwebsocketが無効化されている。そのままでは、[こちら](https://github.com/intersystems/Samples-MQTT-EKG-Devices)の簡易Webアプリ(ハートビートモニタ)は動作しない。

https://github.com/eclipse/mosquitto
> libwebsockets (libwebsockets-dev) - enable with make WITH_WEBSOCKETS=yes

```
sudo apt-get install build-essential libwebsockets-dev
git clone https://github.com/eclipse/mosquitto
cd mosquitto
make WITH_WEBSOCKETS=yes WITH_DOCS=no WITH_TLS=no WITH_CJSON=no
sudo make WITH_WEBSOCKETS=yes WITH_DOCS=no WITH_TLS=no WITH_CJSON=no install
ls /usr/local/sbin/ -l
total 2072
-rwxr-xr-x 1 root root 2120800 Nov 29 13:47 mosquitto
```


