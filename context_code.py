import os

# 1. 使用 r"" 来表示原始字符串，解决路径报错
root_dir = r"D:\code\镭射拍拍灯（全） (2)" 
output_file = "D:\code\镭射拍拍灯（全） (2)\Project_Code_Context.txt"

# 不需要扫描的文件夹
ignore_dirs = {'.git', 'Library', 'Temp', 'obj', 'Logs', 'Builds', '.vs', '.vscode'}

def read_file_content(filepath):
    """尝试用不同的编码读取文件"""
    encodings = ['utf-8', 'gbk', 'gb18030'] # 优先尝试 utf-8，失败则尝试中文编码
    for enc in encodings:
        try:
            with open(filepath, "r", encoding=enc) as f:
                return f.read(), enc
        except UnicodeDecodeError:
            continue
    return None, None

with open(output_file, "w", encoding="utf-8") as outfile:
    print("开始扫描项目...")
    
    # 写入目录结构概览
    outfile.write("=== PROJECT STRUCTURE ===\n")
    for dirpath, dirnames, filenames in os.walk(root_dir):
        # 移除忽略的文件夹
        dirnames[:] = [d for d in dirnames if d not in ignore_dirs]
        for f in filenames:
            if f.endswith(".cs"): # 只关注 C# 脚本
                full_path = os.path.join(dirpath, f)
                outfile.write(f"{full_path}\n")
    
    outfile.write("\n\n=== CODE CONTENTS ===\n")
    
    # 写入代码内容
    count = 0
    for dirpath, dirnames, filenames in os.walk(root_dir):
        dirnames[:] = [d for d in dirnames if d not in ignore_dirs]
        for f in filenames:
            if f.endswith(".cs"):
                full_path = os.path.join(dirpath, f)
                content, used_encoding = read_file_content(full_path)
                
                if content is not None:
                    outfile.write(f"\n\n--- START OF FILE: {f} (Encoding: {used_encoding}) ---\n")
                    outfile.write(content)
                    outfile.write(f"\n--- END OF FILE: {f} ---\n")
                    count += 1
                else:
                    print(f"警告: 无法读取文件 {f} (可能是二进制或加密文件)，已跳过。")
                    outfile.write(f"\n\n--- FAILED TO READ FILE: {f} ---\n")

print(f"搞定！成功处理了 {count} 个脚本文件。")
print(f"所有代码已保存到: {os.path.abspath(output_file)}")