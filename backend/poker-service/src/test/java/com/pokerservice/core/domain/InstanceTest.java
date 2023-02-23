package com.pokerservice.core.domain;

import java.util.List;
import org.assertj.core.api.Assertions;
import org.junit.jupiter.api.Test;

public class InstanceTest {

    @Test
    void a(){
        // given
        List<Instance> instances = List.of(new Instance(1, 1), new Instance(2, 5), new Instance(3, 7));
        InstanceStorage instanceStorage = new InstanceStorage(instances);

        // when
        List<Instance> list1 = instanceStorage.getInstances();
        Instance instance = list1.get(0);
        instance.setField1(10);

        // then
        Assertions.assertThat(instanceStorage.getById(1).getField1()).isSameAs(10);
    }

    private static class InstanceStorage {

        List<Instance> instances;

        public InstanceStorage(List<Instance> instances) {
            this.instances = instances;
        }

        public List<Instance> getInstances() {
            return instances;
        }

        public Instance getById(int id) {
            return instances.stream()
                .filter(i -> i.getId() == id)
                .findFirst()
                .get();
        }
    }


    private static class Instance{

        private int id;
        private int field1;


        public int getField1() {
            return field1;
        }

        public void setField1(int field1) {
            this.field1 = field1;
        }

        public int getId() {
            return id;
        }

        public Instance(int id, int field1) {
            this.id = id;
            this.field1 = field1;
        }
    }

}
