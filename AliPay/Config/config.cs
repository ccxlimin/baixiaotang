using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// config 的摘要说明
/// </summary>
public class AliPayConfig
{
    public AliPayConfig()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    // 应用ID,您的APPID
    public const string app_id = "2018042660037312";

    // 支付宝网关
    public const string gatewayUrl = "https://openapi.alipay.com/gateway.do";

    // 商户私钥，您的原始格式RSA私钥
    public const string private_key = @"MIIEpAIBAAKCAQEApt9iHmKMBVvdxSrVIVYH86JkG0y3eDNupnoQdfyFfLETtMKNUOw2rBZPDRdbVuXpvb2PebjJHeCZg1d5nR8z5cz3UMaXN19Hr9i6qXlKmO+esu3/WWqXNLW/VQwVXz5wenr0awVvkLgKwLf8vGLF4MGsgXahHYxaGWAaLLpOrIYrB5Wa5kQM17OW60RCHw7S3OtlZspT42Gx1H2XDO2nqFrH56cGbckKcJk3igeNro8pPStr6AexialZunzgv4RWvMjaG2O/qOmQNi3vhE7iZrNyKyLRu882enVdRlcs6LSDGG4IYDgwnzG/LTiS5z6bDUADZUc8N+x+nQb4tWjnKwIDAQABAoIBAGETQHQmpggUCi5CHZx3/MY5qPzMo0q0uOD1z4+jqFACf1E/gJAb6i4wCCq4dLVXqKNtnAKWgcD0wLlMcP37gehkVVdTP/LBZeWhMe2XFtNjSjITC+ILhQzv0e96yRAQNcS6tGdx0RYiSncUUV2SJET8xgrGZTHYTS3H31I3y77Gmbl5IACmUjJqvsVueAoeX9c83/rc//ULb3CoqrfMD3ZRqxJ3AwR/Bo478/lVdQATJXrHmxk5j3q8jqcDlWTUxvq0ohgCct+t2M8aCWlClyG98YVz84h5Z4SkJnXnZHkTRbfn9wV1jqXBldm6PKfiHMcRnVzbGVx3SvbhWHEM4ZECgYEA1QmmmSbtzr1OeIPXx8MUbghZic0w3bKS/MkmugKJbIZFAezz6vXkW51gdVdXIVbEcB/VzDVievslYd+tygfM+YAAedvUVSrGvFLZ+Kd0MVMH/Jg+dCiygw7f1wf9RSKTyKHU6s8kucbEfk76OJhM68c9l0Fd67WvzeayxFuP0jMCgYEAyIZmCTxiMiVY3qqW/hy8W0VpwX6qrfTV2TPebNIMrPIYBSoLeGCBaWEdTql6GvaYRzS8HKdhWxU9de1XD01AsFzjeKpf+GbG/JmS/Ey3ILNfeuVcB30YSFiynK1cSVzvq/i9J3D2Uk03BV3ZpG6GbqLTKvT+By4JUBQ7kN7zzykCgYEAseUbLyyozEicbFp8J52pQ7DJRB8KPTbPMzOtmQphQzRkD5WUoZGvnSkv0Qyb/4T7CD4tRBcYI2Xw82fbMaeN7JKM0zVUe8H6rmvpBJCgoEeblbxz+n+5gWM8W82aGs8i5RjuOR28pz1p+6drnzLemMyJi/LTwO643ZFVmk5zLK0CgYARL7u1FPDRnXPVeceBNhNKu1NKG0IjsLVXbWOl7gbaF9HJN4ZOJe9RidpKMKpffz0j9TgeOJZWiSaAYfwxOozN0NhKMkgRcTWi1sLLTwPvEJChZuERXz5ijP+aAB3Fx+pe0e3cYIksGqQZmKabukFZYCk7KE5RbTetHl7QlttMqQKBgQCI7DE5GjVq85Zlc+V1OnzjoiQIKnYBTGnZNf7mNrQTPTcjkyAnId5afgl4vaoySXPUjuKbFc3n8p4rHwo1/uK1FS7Sw/xi2Lfp08QSRwrLC/Y0YMBVOX2FQd48u7//ehmeh54m9amrJkODsC+KmiVbu3W5/RKXnNF9Qr/hR4z9Kw==";

    // 支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。
    public const string alipay_public_key = @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApIScB0QB4jlWOR/jdSeyJ5bybqNUG2UW4xm1MaEMNCtgA4JhANXuSeivxobGvyt4QQ6xox/GJXr+EiJmZCD+Os7/4zJUOEvsX/wsumOw6jLnATKLp6o9uEmKAyMXWDI/04NzJ/XvLL6FEd34ZraZ2n46wSbLplXWJOlj22fehy/P/iAFFNXwyVQL189Wbsy+8ITISBfRgAYcAZw7NWR+MwR8a3nNyFzyF71aZ06T7sgi2R5xznCr2ymtvF8UwWSvD3Vqf+GtdLDSKa6px0VP7VDM0fFFELZcgewEGEBnqHN59YT9/5hq3/3TgQ9LKciSNhAtFL4rUgTHEY6z6Fd2zQIDAQAB";

    // 签名方式
    public const string sign_type = "RSA2";

    // 编码格式
    public const string charset = "UTF-8";
}