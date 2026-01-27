// src/utils/format.js

// 專門用來把數字變成 "29,900" 這種格式
export const formatPrice = (value) => {
  if (value === undefined || value === null) return '0';
  // 使用內建的 Intl 工具，設定為台灣格式 (zh-TW)
  return new Intl.NumberFormat('zh-TW', {
    style: 'decimal', 
    minimumFractionDigits: 0
  }).format(value);
};