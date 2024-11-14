#include <bits/stdc++.h>
using namespace std;
using vvi = vector<vector<int>>;

const vector<int> dx = {0, 1, 0, -1};
const vector<int> dy = {1, 0, -1, 0};

vector<pair<int, int>> get_connected(vvi &data)
{
    int h = data.size(), w = data[0].size();
    int sx, sy;
    for(int i = 0; i < h; i++) for(int j = 0; j < w; j++) if(data[i][j] == 2) sx = i, sy = j;
    vector<pair<int, int>> res;
    queue<pair<int, int>> que;

    que.push({sx, sy});
    vector<vector<bool>> visited(h, vector<bool>(w, false));
    visited[sx][sy] = true;
    while(!que.empty())
    {
        auto [x, y] = que.front(); que.pop();
        res.push_back({x, y});
        for(int i = 0; i < 4; i++)
        {
            int nx = x + dx[i], ny = y + dy[i];
            if(nx < 0 || nx >= h || ny < 0 || ny >= w) continue;
            if(data[nx][ny] <= 1) continue;
            if(visited[nx][ny]) continue;
            visited[nx][ny] = true;
            que.push({nx, ny});
        }
    }
    return res;
}

bool can_move(vvi &data, int dir)
{
    int h = data.size(), w = data[0].size();
    auto connected = get_connected(data);
    for(auto [x, y] : connected)
    {
        int nx = x + dx[dir], ny = y + dy[dir];
        if(nx < 0 || nx >= h || ny < 0 || ny >= w) return false;
        if(data[nx][ny] == 0) return false;
    }
    return true;
}

void move(vvi &data, int dir)
{
    if(!can_move(data, dir)) return;
    auto connected = get_connected(data);
    vector<vector<int>> new_data = data;
    for(auto [x, y] : connected) new_data[x][y] = 1;
    for(auto [x, y] : connected)
    {
        int nx = x + dx[dir], ny = y + dy[dir];
        new_data[nx][ny] = data[x][y];
    }
    data = new_data;
    return;
}

bool is_completed(vvi &data)
{
    auto connected = get_connected(data);
    int num = 0;
    for(auto p : data) for(auto q : p) if(q > 1) num++;
    return num == connected.size();
}

void dump(vvi &data)
{
    int h = data.size(), w = data[0].size();
    cout << h << ' ' << w << '\n';
    for(auto p : data)
    {
        for(int i = 0; i < w; i++)
        {
            cout << p[i];
            if(i < w - 1) cout << ' ';
        }
        cout << '\n';
    }
}

vector<pair<int, int>> get_list(vvi &data)
{
    int h = data.size(), w = data[0].size();
    vector<pair<int, int>> res;
    int sx, sy; for(int i = 0; i < h; i++) for(int j = 0; j < w; j++) if(data[i][j] == 2) sx = i, sy = j;
    auto connected = get_connected(data);
    for(auto [x, y] : connected) res.emplace_back(x - sx, y - sy);
    sort(res.begin(), res.end());
    return res;
}

int ran()
{
    random_device seed_gen;
    mt19937 engine(seed_gen());
    uniform_int_distribution<int> dist(0, 100);
    return dist(engine);
}

vvi make_grid(int h, int w)
{
    vector<vector<int>> grid(h, vector<int>(w, 0));
    for(int i = 1; i < h - 1; i++) for(int j = 1; j < w - 1; j++) grid[i][j] = 1;
    grid[ran() % (h - 2) + 1][ran() % (w - 2) + 1] = 2;
    for(int i = 1; i < h - 1; i++) for(int j = 1; j < w - 1; j++)
    {
        if(grid[i][j] == 2) continue;
        int r = ran();
        if(r < 0) grid[i][j] = 0;
        else if(r < 7) grid[i][j] = 3;
        else if(r < 14) grid[i][j] = 4;
        else if(r < 21) grid[i][j] = 5;
    }
    return grid;
}

void check(int h, int w)
{
    auto data = make_grid(h, w);
    map<vvi, int> dist;
    set<vector<pair<int, int>>> lists;
    queue<pair<vvi, string>> que;
    que.emplace(data, "");
    dist[data] = 0;
    while(!que.empty())
    {
        auto [cur, ans] = que.front(); que.pop();
        if(is_completed(cur))
        {
            auto list = get_list(cur);
            if(lists.count(list)) continue;
            if(dist[cur] < 35) continue;
            cout << "----------------\n";
            cout << "completed : " << dist[cur] << '\n';
            dump(data); cout << "======================\n";
            dump(cur);
            cout << "ans : " << ans << '\n';
            cout << "----------------\n";
            lists.insert(list);
            continue;
        }
        for(int i = 0; i < 4; i++)
        {
            if(!can_move(cur, i)) continue;
            auto next = cur;
            move(next, i);
            if(dist.count(next)) continue;
            string new_ans = ans + "RDLU"[i];
            dist[next] = dist[cur] + 1;
            que.emplace(next, new_ans);
        }
    }
}

int main()
{
    int h, w; cin >> h >> w;
    for(int i = 0; i < 10000; i++) check(h, w);
}