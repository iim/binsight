.class public Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;
.super Ljava/lang/Object;
.source "Cls_CryptoFunz.java"


# instance fields
.field private m_MasterKey:[B


# direct methods
.method public constructor <init>([B)V
    .locals 1
    .param p1, "p_Key"    # [B

    .prologue
    .line 46
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    .line 48
    invoke-virtual {p1}, [B->clone()Ljava/lang/Object;

    move-result-object v0

    check-cast v0, [B

    iput-object v0, p0, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->m_MasterKey:[B

    .line 49
    return-void
.end method

.method public static getBytesfromHexString(Ljava/lang/String;)[B
    .locals 5
    .param p0, "sInput"    # Ljava/lang/String;

    .prologue
    .line 34
    invoke-virtual {p0}, Ljava/lang/String;->length()I

    move-result v3

    div-int/lit8 v3, v3, 0x2

    new-array v0, v3, [B

    .line 35
    .local v0, "arr":[B
    const/4 v1, 0x0

    .local v1, "start":I
    :goto_0
    invoke-virtual {p0}, Ljava/lang/String;->length()I

    move-result v3

    if-lt v1, v3, :cond_0

    .line 40
    return-object v0

    .line 37
    :cond_0
    add-int/lit8 v3, v1, 0x2

    invoke-virtual {p0, v1, v3}, Ljava/lang/String;->substring(II)Ljava/lang/String;

    move-result-object v2

    .line 38
    .local v2, "thisByte":Ljava/lang/String;
    div-int/lit8 v3, v1, 0x2

    const/16 v4, 0x10

    invoke-static {v2, v4}, Ljava/lang/Byte;->parseByte(Ljava/lang/String;I)B

    move-result v4

    aput-byte v4, v0, v3

    .line 35
    add-int/lit8 v1, v1, 0x2

    goto :goto_0
.end method

.method public static toHexString([B)Ljava/lang/String;
    .locals 6
    .param p0, "bytes"    # [B

    .prologue
    .line 20
    const/16 v4, 0x10

    new-array v0, v4, [C

    fill-array-data v0, :array_0

    .line 21
    .local v0, "hexArray":[C
    array-length v4, p0

    mul-int/lit8 v4, v4, 0x2

    new-array v1, v4, [C

    .line 23
    .local v1, "hexChars":[C
    const/4 v2, 0x0

    .local v2, "j":I
    :goto_0
    array-length v4, p0

    if-lt v2, v4, :cond_0

    .line 28
    new-instance v4, Ljava/lang/String;

    invoke-direct {v4, v1}, Ljava/lang/String;-><init>([C)V

    return-object v4

    .line 24
    :cond_0
    aget-byte v4, p0, v2

    and-int/lit16 v3, v4, 0xff

    .line 25
    .local v3, "v":I
    mul-int/lit8 v4, v2, 0x2

    div-int/lit8 v5, v3, 0x10

    aget-char v5, v0, v5

    aput-char v5, v1, v4

    .line 26
    mul-int/lit8 v4, v2, 0x2

    add-int/lit8 v4, v4, 0x1

    rem-int/lit8 v5, v3, 0x10

    aget-char v5, v0, v5

    aput-char v5, v1, v4

    .line 23
    add-int/lit8 v2, v2, 0x1

    goto :goto_0

    .line 20
    nop

    :array_0
    .array-data 2
        0x30s
        0x31s
        0x32s
        0x33s
        0x34s
        0x35s
        0x36s
        0x37s
        0x38s
        0x39s
        0x41s
        0x42s
        0x43s
        0x44s
        0x45s
        0x46s
    .end array-data
.end method


# virtual methods
.method public decrypt(Ljava/lang/String;)Ljava/lang/String;
    .locals 1
    .param p1, "encrypted"    # Ljava/lang/String;

    .prologue
    .line 120
    iget-object v0, p0, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->m_MasterKey:[B

    invoke-virtual {p0, v0, p1}, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->decrypt([BLjava/lang/String;)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public decrypt([BLjava/lang/String;)Ljava/lang/String;
    .locals 13
    .param p1, "key"    # [B
    .param p2, "encrypted"    # Ljava/lang/String;

    .prologue
    const/4 v9, 0x0

    .line 156
    :try_start_0
    invoke-static {p2}, Lcom/lindberghapps/kidsboard/Cls_Crypto$MatrixEnc;->decode(Ljava/lang/String;)[B

    move-result-object v4

    .line 158
    .local v4, "encBytes":[B
    new-instance v8, Ljavax/crypto/spec/SecretKeySpec;

    const-string v10, "AES"

    invoke-direct {v8, p1, v10}, Ljavax/crypto/spec/SecretKeySpec;-><init>([BLjava/lang/String;)V

    .line 160
    .local v8, "skeySpec":Ljavax/crypto/spec/SecretKeySpec;
    const-string v10, "AES/CBC/PKCS7Padding"

    const-string v11, "BC"

    invoke-static {v10, v11}, Ljavax/crypto/Cipher;->getInstance(Ljava/lang/String;Ljava/lang/String;)Ljavax/crypto/Cipher;

    move-result-object v0

    .line 161
    .local v0, "cipher":Ljavax/crypto/Cipher;
    const/16 v10, 0x10

    new-array v6, v10, [B

    .line 162
    .local v6, "iv":[B
    new-instance v7, Ljavax/crypto/spec/IvParameterSpec;

    invoke-direct {v7, v6}, Ljavax/crypto/spec/IvParameterSpec;-><init>([B)V

    .line 163
    .local v7, "ivSpec":Ljavax/crypto/spec/IvParameterSpec;
    const/4 v10, 0x2

    invoke-virtual {v0, v10, v8, v7}, Ljavax/crypto/Cipher;->init(ILjava/security/Key;Ljava/security/spec/AlgorithmParameterSpec;)V

    .line 164
    invoke-virtual {v0, v4}, Ljavax/crypto/Cipher;->doFinal([B)[B

    move-result-object v2

    .line 165
    .local v2, "decBytes":[B
    new-instance v3, Ljava/lang/String;

    const/4 v10, 0x2

    array-length v11, v2

    add-int/lit8 v11, v11, -0x2

    const-string v12, "UTF8"

    invoke-direct {v3, v2, v10, v11, v12}, Ljava/lang/String;-><init>([BIILjava/lang/String;)V
    :try_end_0
    .catch Ljavax/crypto/BadPaddingException; {:try_start_0 .. :try_end_0} :catch_0
    .catch Ljava/lang/Exception; {:try_start_0 .. :try_end_0} :catch_1

    .line 176
    .end local v0    # "cipher":Ljavax/crypto/Cipher;
    .end local v2    # "decBytes":[B
    .end local v4    # "encBytes":[B
    .end local v6    # "iv":[B
    .end local v7    # "ivSpec":Ljavax/crypto/spec/IvParameterSpec;
    .end local v8    # "skeySpec":Ljavax/crypto/spec/SecretKeySpec;
    :goto_0
    return-object v3

    .line 167
    :catch_0
    move-exception v1

    .local v1, "cryptoEx":Ljavax/crypto/BadPaddingException;
    move-object v3, v9

    .line 168
    goto :goto_0

    .line 171
    .end local v1    # "cryptoEx":Ljavax/crypto/BadPaddingException;
    :catch_1
    move-exception v5

    .line 172
    .local v5, "ex":Ljava/lang/Exception;
    invoke-virtual {v5}, Ljava/lang/Exception;->printStackTrace()V

    move-object v3, v9

    .line 173
    goto :goto_0
.end method

.method public decrypt_fromHEX(Ljava/lang/String;)Ljava/lang/String;
    .locals 4
    .param p1, "encryptedHex"    # Ljava/lang/String;

    .prologue
    .line 130
    const-string v1, ""

    .line 134
    .local v1, "sTmpEnc":Ljava/lang/String;
    :try_start_0
    new-instance v2, Ljava/lang/String;

    invoke-static {p1}, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->getBytesfromHexString(Ljava/lang/String;)[B

    move-result-object v3

    invoke-direct {v2, v3}, Ljava/lang/String;-><init>([B)V
    :try_end_0
    .catch Ljava/lang/Exception; {:try_start_0 .. :try_end_0} :catch_0

    .line 136
    .end local v1    # "sTmpEnc":Ljava/lang/String;
    .local v2, "sTmpEnc":Ljava/lang/String;
    :try_start_1
    iget-object v3, p0, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->m_MasterKey:[B

    invoke-virtual {p0, v3, v2}, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->decrypt([BLjava/lang/String;)Ljava/lang/String;
    :try_end_1
    .catch Ljava/lang/Exception; {:try_start_1 .. :try_end_1} :catch_1

    move-result-object v1

    .line 145
    .end local v2    # "sTmpEnc":Ljava/lang/String;
    .restart local v1    # "sTmpEnc":Ljava/lang/String;
    :goto_0
    return-object v1

    .line 140
    :catch_0
    move-exception v0

    .line 142
    .local v0, "exc":Ljava/lang/Exception;
    :goto_1
    const-string v1, ""

    goto :goto_0

    .line 140
    .end local v0    # "exc":Ljava/lang/Exception;
    .end local v1    # "sTmpEnc":Ljava/lang/String;
    .restart local v2    # "sTmpEnc":Ljava/lang/String;
    :catch_1
    move-exception v0

    move-object v1, v2

    .end local v2    # "sTmpEnc":Ljava/lang/String;
    .restart local v1    # "sTmpEnc":Ljava/lang/String;
    goto :goto_1
.end method

.method public encrypt(Ljava/lang/String;)Ljava/lang/String;
    .locals 1
    .param p1, "clear"    # Ljava/lang/String;

    .prologue
    .line 62
    iget-object v0, p0, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->m_MasterKey:[B

    invoke-virtual {p0, v0, p1}, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->encrypt([BLjava/lang/String;)Ljava/lang/String;

    move-result-object v0

    return-object v0
.end method

.method public encrypt([BLjava/lang/String;)Ljava/lang/String;
    .locals 10
    .param p1, "key"    # [B
    .param p2, "clear"    # Ljava/lang/String;

    .prologue
    .line 96
    const/4 v8, 0x2

    new-array v6, v8, [B

    .line 98
    .local v6, "salt":[B
    new-instance v7, Ljavax/crypto/spec/SecretKeySpec;

    const-string v8, "AES"

    invoke-direct {v7, p1, v8}, Ljavax/crypto/spec/SecretKeySpec;-><init>([BLjava/lang/String;)V

    .line 100
    .local v7, "skeySpec":Ljavax/crypto/spec/SecretKeySpec;
    :try_start_0
    new-instance v5, Ljava/util/Random;

    invoke-direct {v5}, Ljava/util/Random;-><init>()V

    .line 101
    .local v5, "rnd":Ljava/util/Random;
    const-string v8, "AES/CBC/PKCS7Padding"

    const-string v9, "BC"

    invoke-static {v8, v9}, Ljavax/crypto/Cipher;->getInstance(Ljava/lang/String;Ljava/lang/String;)Ljavax/crypto/Cipher;

    move-result-object v0

    .line 102
    .local v0, "cipher":Ljavax/crypto/Cipher;
    const/16 v8, 0x10

    new-array v3, v8, [B

    .line 103
    .local v3, "iv":[B
    new-instance v4, Ljavax/crypto/spec/IvParameterSpec;

    invoke-direct {v4, v3}, Ljavax/crypto/spec/IvParameterSpec;-><init>([B)V

    .line 104
    .local v4, "ivSpec":Ljavax/crypto/spec/IvParameterSpec;
    const/4 v8, 0x1

    invoke-virtual {v0, v8, v7, v4}, Ljavax/crypto/Cipher;->init(ILjava/security/Key;Ljava/security/spec/AlgorithmParameterSpec;)V

    .line 105
    invoke-virtual {v5, v6}, Ljava/util/Random;->nextBytes([B)V

    .line 106
    invoke-virtual {v0, v6}, Ljavax/crypto/Cipher;->update([B)[B

    .line 107
    invoke-virtual {p2}, Ljava/lang/String;->getBytes()[B

    move-result-object v8

    invoke-virtual {v0, v8}, Ljavax/crypto/Cipher;->doFinal([B)[B
    :try_end_0
    .catch Ljava/lang/Exception; {:try_start_0 .. :try_end_0} :catch_0

    move-result-object v1

    .line 113
    .local v1, "encrypted":[B
    invoke-static {v1}, Lcom/lindberghapps/kidsboard/Cls_Crypto$MatrixEnc;->encode([B)Ljava/lang/String;

    move-result-object v8

    .end local v0    # "cipher":Ljavax/crypto/Cipher;
    .end local v1    # "encrypted":[B
    .end local v3    # "iv":[B
    .end local v4    # "ivSpec":Ljavax/crypto/spec/IvParameterSpec;
    .end local v5    # "rnd":Ljava/util/Random;
    :goto_0
    return-object v8

    .line 108
    :catch_0
    move-exception v2

    .line 109
    .local v2, "ex":Ljava/lang/Exception;
    invoke-virtual {v2}, Ljava/lang/Exception;->printStackTrace()V

    .line 110
    const/4 v8, 0x0

    goto :goto_0
.end method

.method public encrypt_asHex(Ljava/lang/String;)Ljava/lang/String;
    .locals 4
    .param p1, "clear"    # Ljava/lang/String;

    .prologue
    .line 77
    const-string v1, ""

    .line 79
    .local v1, "sRes":Ljava/lang/String;
    :try_start_0
    iget-object v3, p0, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->m_MasterKey:[B

    invoke-virtual {p0, v3, p1}, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->encrypt([BLjava/lang/String;)Ljava/lang/String;

    move-result-object v2

    .line 81
    .local v2, "tmpString":Ljava/lang/String;
    invoke-virtual {v2}, Ljava/lang/String;->getBytes()[B

    move-result-object v3

    invoke-static {v3}, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->toHexString([B)Ljava/lang/String;
    :try_end_0
    .catch Ljava/lang/Exception; {:try_start_0 .. :try_end_0} :catch_0

    move-result-object v1

    .line 89
    .end local v2    # "tmpString":Ljava/lang/String;
    :goto_0
    return-object v1

    .line 84
    :catch_0
    move-exception v0

    .line 86
    .local v0, "exc":Ljava/lang/Exception;
    const-string v1, ""

    goto :goto_0
.end method

.method public setMasterKey([B)V
    .locals 1
    .param p1, "p_Key"    # [B

    .prologue
    .line 70
    invoke-virtual {p1}, [B->clone()Ljava/lang/Object;

    move-result-object v0

    check-cast v0, [B

    iput-object v0, p0, Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->m_MasterKey:[B

    .line 71
    return-void
.end method
