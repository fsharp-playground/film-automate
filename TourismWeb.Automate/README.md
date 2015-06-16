### Setup Remote Server

http://www.guru99.com/introduction-to-selenium-grid.html

```
java -jar selenium-server-standalone-2.4.6.0.jar -webdriver.chome=~/Command/chromedriver -role hub
```

### Test Server

```
http://10.0.0.146:4444/grid/console
```

### Register

```
java -jar selenium-standalong-2.46.0.jar -role webdriver -hub http://10.0.0.25:4444/grid/register -port 5566
```


### Check Registration

```
http://10.0.0.146:4444/grid/console
```

### Enable Report Request

```
iisexpress-proxy 51123 3000
```

