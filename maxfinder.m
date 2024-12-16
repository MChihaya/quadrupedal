% JSONファイルが保存されているディレクトリを指定
dirPath = 'SaveData/forwardleft'; % ここを対象フォルダに置き換えてください

filePattern = fullfile(dirPath, 'best_robots_save_data_*.json'); % ファイル名パターン
jsonFiles = dir(filePattern); % パターンに一致するファイル一覧を取得

% 初期化
maxRewards = [-inf, -inf, -inf]; % 上位3つのreward値を記録
bestEntries = cell(1, 3);        % 対応するエントリを保存
bestFiles = cell(1, 3);          % 対応するファイル名を保存

% 各ファイルを処理
for i = 1:length(jsonFiles)
    % ファイルのフルパス
    filePath = fullfile(jsonFiles(i).folder, jsonFiles(i).name);
    
    % JSONファイルを読み込む
    jsonData = jsondecode(fileread(filePath));
    
    % "geneDatas"フィールドの確認と取得
    if isfield(jsonData, 'geneDatas')
        geneDatas = jsonData.geneDatas;
        
        % reward値を抽出
        rewards = arrayfun(@(x) x.reward, geneDatas);
        
        % 各rewardを処理して上位3つを更新
        for j = 1:length(rewards)
            currentReward = rewards(j);
            if currentReward > min(maxRewards)
                % 最小値を置き換える
                [~, minIndex] = min(maxRewards);
                maxRewards(minIndex) = currentReward;
                bestEntries{minIndex} = geneDatas(j);
                bestFiles{minIndex} = filePath;
            end
        end
    else
        fprintf('警告: ファイル "%s" に "geneDatas" フィールドが存在しません。\n', filePath);
    end
end

% 最大値でソート
[maxRewards, sortIdx] = sort(maxRewards, 'descend');
bestEntries = bestEntries(sortIdx);
bestFiles = bestFiles(sortIdx);

% 結果の表示
for rank = 1:3
    if ~isempty(bestEntries{rank})
        fprintf('Top %d の reward 値: %f\n', rank, maxRewards(rank));
        fprintf('対応するデータ:\n');
        disp(bestEntries{rank});
        fprintf('ファイル名: %s\n', bestFiles{rank});
    else
        fprintf('Top %d のデータは見つかりませんでした。\n', rank);
    end
end