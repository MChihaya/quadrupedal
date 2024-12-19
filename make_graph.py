import json
import glob
import matplotlib.pyplot as plt
from collections import defaultdict

# 1. JSONファイルの読み込み関数
def load_json_files(path_pattern):
    all_data = []
    file_list = glob.glob(path_pattern)
    print(f"Found {len(file_list)} files.")
    
    for file in file_list:
        with open(file, 'r', encoding='utf-8') as f:
            data = json.load(f)
            all_data.extend(data['geneDatas'])  # geneDatasの内容をリストに追加
    return all_data

# 2. データを抽出し、best/second best/third bestを分ける関数
def extract_ranked_rewards(data):
    generation_rewards = defaultdict(list)
    
    # generationごとにデータをまとめる（ソート済みなので順番に取得）
    for item in data:
        generation = item.get('generation', 0)
        reward = item.get('reward', 0)
        generation_rewards[generation].append(reward)
    
    # best, second best, third best を順番に取得
    generations = []
    best_rewards = []
    second_rewards = []
    third_rewards = []
    
    for generation, rewards in sorted(generation_rewards.items()):
        generations.append(generation)
        best_rewards.append(rewards[0] if len(rewards) > 0 else None)  # 1つ目のreward
        second_rewards.append(rewards[1] if len(rewards) > 1 else None)  # 2つ目のreward
        third_rewards.append(rewards[2] if len(rewards) > 2 else None)  # 3つ目のreward
    
    return generations, best_rewards, second_rewards, third_rewards

# 3. グラフを描画する関数
def plot_graph(generations, best, second, third):
    plt.figure(figsize=(10, 6))
    plt.plot(generations, best, 'o-', label='Best', color='gold', markersize=6)
    plt.plot(generations, second, 's-', label='Second Best', color='silver', markersize=6)
    plt.plot(generations, third, '^-', label='Third Best', color='brown', markersize=6)
    
    plt.title("Highly rewarded robots")
    # plt.xlim([0, 40])
    plt.xlabel("Generation")
    plt.ylabel("Reward")
    plt.legend()
    plt.show()

# 4. メイン処理
if __name__ == "__main__":
    # ファイル名パターンを指定（例: カレントディレクトリ内のbest_robots_save_data_*.json）
    file_pattern = "./SaveData/neuro8wards4/best_robots_save_data_*.json"
    
    # JSONファイルを読み込む
    json_data = load_json_files(file_pattern)
    
    # best, second best, third bestを抽出する
    generations, best_rewards, second_rewards, third_rewards = extract_ranked_rewards(json_data)
    
    # グラフを描画する
    plot_graph(generations, best_rewards, second_rewards, third_rewards)
