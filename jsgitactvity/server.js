const express = require('express');
const axios = require('axios');

const app = express();
const PORT = 5000;

app.get('/user/:username', async (req, res) => {
    const { username } = req.params;
    const url = `https://api.github.com/users/${username}/events`;

    try {
        const response = await axios.get(url);
        const data = response.data;

        // Process activity
        const activity = data.map(event => {
            switch (event.type) {
                case 'PushEvent':
                    const repo = event.repo.name;
                    const commits = event.payload.commits.length;
                    return `Pushed ${commits} commit(s) to ${repo}`;
                case 'IssuesEvent':
                    const action = event.payload.action;
                    return `${action.charAt(0).toUpperCase() + action.slice(1)} an issue in ${event.repo.name}`;
                case 'WatchEvent':
                    return `Starred ${event.repo.name}`;
                default:
                    return `Performed ${event.type} on ${event.repo.name}`;
            }
        });

        res.json(activity);

    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Error fetching GitHub activity' });
    }
});

app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});
