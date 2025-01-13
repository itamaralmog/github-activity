const axios = require('axios');

const fetchGitHubActivity = async (username) => {
    const url = `http://localhost:5000/user/${username}`;
    try {
        const response = await axios.get(url);
        const activity = response.data;

        if (activity.length === 0) {
            console.log(`No recent activity found for user '${username}'.`);
        } else {
            console.log(`GitHub Activity for user '${username}':`);
            activity.forEach(act => console.log(`- ${act}`));
        }
    } catch (error) {
        console.error(`Error fetching activity for user '${username}':`, error.response?.data || error.message);
    }
};

const args = process.argv.slice(2);
if (args.length !== 1) {
    console.log('Usage: node cli.js <username>');
} else {
    const username = args[0];
    fetchGitHubActivity(username);
}
