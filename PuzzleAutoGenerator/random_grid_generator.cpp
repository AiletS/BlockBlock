#include <bits/stdc++.h>
using namespace std;

int ran()
{
    random_device seed_gen;
    mt19937 engine(seed_gen());
    uniform_int_distribution<int> dist(0, 100);
    return dist(engine);
}

int main()
{
    int h, w; cin >> h >> w;
    vector<vector<int>> grid(h, vector<int>(w, 0));
    for(int i = 1; i < h - 1; i++) for(int j = 1; j < w - 1; j++) grid[i][j] = 1;
    grid[ran() % (h - 2) + 1][ran() % (w - 2) + 1] = 2;
    for(int i = 1; i < h - 1; i++) for(int j = 1; j < w - 1; j++)
    {
        if(grid[i][j] == 2) continue;
        int r = ran();
        if(r < 15) grid[i][j] = 0;
        else if(r < 25) grid[i][j] = 3;
        else if(r < 35) grid[i][j] = 4;
    }

    for(int i = 0; i < h; i++)
    {
        for(int j = 0; j < w; j++)
        {
            cout << grid[i][j];
            if(j < w - 1) cout << " ";
        }
        cout << '\n';
    }
}