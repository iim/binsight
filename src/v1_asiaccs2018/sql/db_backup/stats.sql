SELECT 
  BIO_COUNT = (SELECT COUNT(*) FROM BinaryObject),
  APK_TODECODE = (SELECT COUNT(*) FROM BinaryObject WHERE bioIsRoot = 1 AND bioProcessingStage IN (0)),
  APK_PROCESSING = (SELECT COUNT(*) FROM BinaryObject WHERE bioIsRoot = 1 AND bioProcessingStage IN (1)),
  APK_DECODED = (SELECT COUNT(*) FROM BinaryObject WHERE bioIsRoot = 1 AND bioProcessingStage IN (2)),
  BOC_COUNT = (SELECT COUNT(*) FROM BinaryObjectContent),
  BOP_COUNT = (SELECT COUNT(*) FROM BinaryObjectPath),
  SMALI_COUNT = (SELECT COUNT(*) FROM BinaryObject WHERE bioFileName LIKE '%.smali'),
  JAVATYPES = (SELECT COUNT(*) FROM JavaType)
