from django.db import models
from django.utils import timezone


class User(models.Model):
    id = models.PositiveIntegerField(primary_key=True)
    nickname = models.CharField(max_length=150)


class Room(models.Model):
    name = models.CharField(max_length=150)
    online = models.ManyToManyField(to=User, blank=True, related_name='rooms')

    def get_online_count(self):
        return self.online.count()

    def join(self, user):
        self.online.add(user)
        self.save()

    def leave(self, user):
        self.online.remove(user)
        self.save()

    def __str__(self):
        return f'{self.name} ({self.get_online_count()})'

    class Meta:
        db_table = "Room"
