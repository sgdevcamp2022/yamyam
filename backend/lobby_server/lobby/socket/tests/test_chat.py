from channels.testing import ChannelsLiveServerTestCase
from selenium import webdriver


class SocketTests(ChannelsLiveServerTestCase):
    serve_static = True

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        try:
            cls.driver = webdriver.Chrome()
        except:
            super().tearDownClass()
            raise

    @classmethod
    def tearDownClass(cls):
        cls.driver.quit()
        super().tearDownClass()
