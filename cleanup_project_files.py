#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Unity 项目文件清理脚本
清理项目根目录下的冗余 Visual Studio 解决方案文件（.sln）和项目文件（.csproj）
"""

import os
import sys
from pathlib import Path

def find_project_files(root_dir):
    """
    查找所有 .sln 和 .csproj 文件
    
    Args:
        root_dir: 项目根目录路径
        
    Returns:
        tuple: (sln_files, csproj_files) 两个列表
    """
    root_path = Path(root_dir)
    sln_files = []
    csproj_files = []
    
    # 查找所有 .sln 文件
    for sln_file in root_path.glob("*.sln"):
        sln_files.append(sln_file)
    
    # 查找所有 .csproj 文件
    for csproj_file in root_path.glob("*.csproj"):
        csproj_files.append(csproj_file)
    
    return sln_files, csproj_files

def print_file_list(sln_files, csproj_files):
    """
    打印要删除的文件列表
    
    Args:
        sln_files: .sln 文件列表
        csproj_files: .csproj 文件列表
    """
    print("=" * 80)
    print("发现以下 Visual Studio 项目文件：")
    print("=" * 80)
    
    if sln_files:
        print(f"\n【.sln 文件】共 {len(sln_files)} 个：")
        for i, sln_file in enumerate(sln_files, 1):
            print(f"  {i}. {sln_file.name}")
    
    if csproj_files:
        print(f"\n【.csproj 文件】共 {len(csproj_files)} 个：")
        for i, csproj_file in enumerate(csproj_files, 1):
            print(f"  {i}. {csproj_file.name}")
    
    total_count = len(sln_files) + len(csproj_files)
    print(f"\n总计：{total_count} 个文件")
    print("=" * 80)

def delete_files(sln_files, csproj_files, keep_unity_files=True):
    """
    删除文件
    
    Args:
        sln_files: .sln 文件列表
        csproj_files: .csproj 文件列表
        keep_unity_files: 是否保留 Unity 自动生成的文件（Assembly-CSharp*.csproj）
        
    Returns:
        tuple: (deleted_count, failed_files) 删除的文件数量和失败的文件列表
    """
    deleted_count = 0
    failed_files = []
    
    all_files = sln_files + csproj_files
    
    for file_path in all_files:
        # 如果启用保留 Unity 文件选项，跳过 Unity 自动生成的文件
        if keep_unity_files and file_path.name.startswith("Assembly-CSharp"):
            print(f"⏭️  跳过 Unity 自动生成文件: {file_path.name}")
            continue
        
        try:
            file_path.unlink()  # 删除文件
            print(f"✅ 已删除: {file_path.name}")
            deleted_count += 1
        except Exception as e:
            print(f"❌ 删除失败: {file_path.name} - 错误: {str(e)}")
            failed_files.append((file_path.name, str(e)))
    
    return deleted_count, failed_files

def main():
    """
    主函数
    """
    # 获取脚本所在目录作为项目根目录
    script_dir = Path(__file__).parent.absolute()
    root_dir = script_dir
    
    print("Unity 项目文件清理脚本")
    print("=" * 80)
    print(f"项目根目录: {root_dir}")
    print()
    
    # 查找所有项目文件
    sln_files, csproj_files = find_project_files(root_dir)
    
    if not sln_files and not csproj_files:
        print("✅ 未发现任何 .sln 或 .csproj 文件，无需清理。")
        return
    
    # 打印文件列表
    print_file_list(sln_files, csproj_files)
    
    # 询问用户是否保留 Unity 自动生成的文件
    print("\n⚠️  注意：")
    print("  - Assembly-CSharp.csproj 和 Assembly-CSharp-Editor.csproj")
    print("    是 Unity 自动生成的项目文件，通常可以保留（Unity 会自动重新生成）")
    print("  - 其他 .sln 和 .csproj 文件通常是历史遗留文件，可以安全删除")
    
    # 询问用户确认
    print("\n" + "=" * 80)
    response = input("是否删除上述文件？(yes/no，默认: no): ").strip().lower()
    
    if response not in ['yes', 'y', '是']:
        print("❌ 操作已取消。")
        return
    
    # 询问是否保留 Unity 文件
    keep_unity = input("\n是否保留 Unity 自动生成的文件 (Assembly-CSharp*.csproj)？(yes/no，默认: yes): ").strip().lower()
    keep_unity_files = keep_unity not in ['no', 'n', '否']
    
    print("\n开始删除文件...")
    print("-" * 80)
    
    # 执行删除
    deleted_count, failed_files = delete_files(sln_files, csproj_files, keep_unity_files)
    
    # 打印结果
    print("-" * 80)
    print(f"\n✅ 成功删除 {deleted_count} 个文件")
    
    if failed_files:
        print(f"\n❌ 删除失败 {len(failed_files)} 个文件：")
        for file_name, error in failed_files:
            print(f"  - {file_name}: {error}")
    
    print("\n清理完成！")

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\n\n❌ 操作被用户中断。")
        sys.exit(1)
    except Exception as e:
        print(f"\n❌ 发生错误: {str(e)}")
        import traceback
        traceback.print_exc()
        sys.exit(1)
