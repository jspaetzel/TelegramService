# Telegram Api Service
This is a small service that integrates directly with the Telegram TL-schema api and exposes many of the methods in a friendly RESTful format.

# Dependencies
Mono 4.6.2 or higher. Tested on Apache with mod_mono.

# Status
This project is unmaintained. I created this for enl.io's Telegram integration but we've since moved to using the official http based [Telegram Bot Api](https://core.telegram.org/bots) and [MadelineProto](https://github.com/danog/MadelineProto) for features not supported by the bot api.

# Methods
The methods available in this api are briefly listed below and in more detail at /swagger.

## Channels
* POST /channels
* POST /channels/{channelId}/users/{userId}/role
* GET /channels/{channelId}
* POST /channels/invite
* POST /channels/remove
* GET /channels/{channelId}/history

## Chats
* POST /chats
* POST /chats/{chatId}/admin-toggle
* POST /chats/{chatId}/send-message
* POST /chats/{chatId}/is-admin
* POST /chats/{chatId}/add
* POST /chats/{chatId}/remove
* POST /chats/{chatId}/rename
* GET /chats/{chatId}/history
* GET /chats/{chatId}/members
* POST /chats/{chatId}/migrate

## Config
These endpoints are just for getting the api connected and checking it's status.
* GET /config/status
* GET /config/signin
* POST /config/verify

## Users
* GET /users/contacts
* GET /users/chats
* GET /users
* GET /users/{userId}/history
