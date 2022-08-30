# dsb-bad-dog

Service that automatically notify you and provide the required data to get compensation tickets from DSB's "Travel Time Guarantee" when your train gets delayed

### Still in development

---

#### To Do

- [x] Setup Azure MySQL database
- [x] Create Azure Webhook to save receipt info to db
- [x] Create Azure function to check for delayed train rides using rejseplanen API
- [ ] Send notify email to user using Mailjet when train delayed

---

DSB's trains are sometimes (read way too often) delayed. Conveniently they have a "Travel Time Guarantee" that oblige if your train is more than 30 mins delayed.
That's very smart you'd think: "When my train is more than 30 mins delayed it automatically gives me a new one" - nope! That would be way to easy.
DSB makes you fill out a way to complicated form with details such as when the train should have been arriving, the actual arrival time and proof of ticket purchase - all information they already possess themselves.

Presenting dsb_bad_dog  
A new innovative solution to a problem that shouldn’t exist!
The service will monitor your travels. If a train gets more than 30 minutes delayed, the service sends you an email with all the info required for you to get your compensation ticket.

Because of limited free tiers on email services with API access I'm not able to provide this service for others without your own [Parseur](https://parseur.com/) account. Read [Setup for yourself](#setup-for-yourself]) if you're interested in accessing the service anyways.

## Setup for yourself

This service works by receiving info about a train ride using a webhook. The train ride will then be monitored for delays.

[Parseur](https://parseur.com/) is a platform that makes extracting email data automatically easy. The data can then be sent to this service from Parseur using a webhook. When you recieve your train receipt from netbutikken@dsb.dk you want to forward the email to your Parseur email inbox which will extract the relevant data and send it to this service.

If you would like to use this service please do the following:

- Create a free account on [Parseur](https://parseur.com/)
- Make a new template on Parseur for extracting data from a DSB ticket with the following fields:

```
depStation: Text,
arrStation: Text,
date: Text,
depTime: Time,
arrTime: Time,
price: Number,
orderId: Text,
```

- Setup a inbox rule for your personal email to forward all emails from netbutikken@dsb.dk to your Parseur email inbox
- Contact me and get the endpoint to connect Parseur to the service's webhook
