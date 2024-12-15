% JSONファイルが保存されているディレクトリを指定
dirPath = 'SaveData/forwardleft'; % ここを対象フォルダに置き換えてください

filePattern = fullfile(dirPath, 'best_robots_save_data_*.json'); % ファイル名パターン
jsonFiles = dir(filePattern); % パターンに一致するファイル一覧を取得

% 初期化
overallMaxReward = -inf; % 最も大きなreward値を記録
overallBestEntry = []; % 対応するエントリ
overallBestFile = ''; % 対応するファイル名

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
        
        % ファイル内の最大reward値とそのインデックスを取得
        [fileMaxReward, fileMaxIndex] = max(rewards);
        
        % 全体の最大値と比較・更新
        if fileMaxReward > overallMaxReward
            overallMaxReward = fileMaxReward;
            overallBestEntry = geneDatas(fileMaxIndex);
            overallBestFile = filePath;
        end
    else
        fprintf('警告: ファイル "%s" に "geneDatas" フィールドが存在しません。\n', filePath);
    end
end

% 結果の表示
if ~isempty(overallBestEntry)
    fprintf('全ファイル中の最大のreward値: %f\n', overallMaxReward);
    fprintf('対応するデータ:\n');
    disp(overallBestEntry);
    fprintf('ファイル名: %s\n', overallBestFile);
else
    fprintf('適切なデータが見つかりませんでした。\n');
end